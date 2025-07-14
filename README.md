# Horde Slayer Design Document
> Autor: Santiago Cabo
> 
![Screenshot del juego donde se muestra el inventario abierto, mostrando la interface](https://storage.googleapis.com/santi-documents/ScreenJuego.png)
Este Major Update es una ampliación bastante ambiciosa del Juego. La mayor característica que se buscó es la facilidad para implementar más features mediante sistemas modulares y extendibles basados en interfaces.

## Sistema de inventario

Se amplía la pantalla de personaje para incluir el inventario.
### Características
-   Es posible marcar cualquier objeto del juego como `Item` (Objeto recolectable por el Jugador)
-   Se le puede asignar rareza a un `Item` (Common, Uncommon, Rare, Legendary)
-  Cuando el `Item` está en el piso, muestra un efecto visual en base a su rareza.
-  Es posible configurar un `Item` para que sea “guardable” en el inventario.
-  El inventario muestra todos los `Item` que contiene.
-   Para cada `Item`, si se le pasa el mouse se muestra `Panel de Info,` y si se clickea, se muestran posibles `Acciones` sobre ese objeto.
-   Todos los `Items` guardables tienen la `Acción` por default de poder tirarse al piso. (se reutiliza la funcionalidad que tenía el swap de armas)
-   Armas y Hechizos son `Item` y se pueden cargar múltiples instancias de cada uno.

## Power Ups mediante Sistema de Slots

Para los Items del punto anterior, se habilita la opción de asignarle slots, donde se pueden insertar mejoras. Estas mejoras son, a su vez, otros items.
### Características
- Se puede marcar un `Item` como `Efecto Insertable`. (OnHit, OnKill, Passive)
- Se puede marcar un `Item` como `Contenedor de Slots`.  (OnHit, OnKill, Passive)
- Mediante inventario es posible arrastrar los `Efectos Insertables`.
- Si se suelta el `Insertable` sobre un `Contenedor`, el `Insertable` se elimina del inventario y el `Item Contenedor` gana un bonus.
- Si el `Item` es un `Contenedor de Slots`, su `Panel de Info` muestra los Slots.
- Si el `Item` es un `Contenedor de Slots`, por cada Efecto Insertado, se muestra un efecto visual cuando está en piso.

### Implementados
- **On Hit Effect**:  Golpe crítico.
- **On Hit Effect**:  Prender fuego a los enemigos.
- **On Hit Effect**:  Knockback.
- **On Kill Effect**:  Ganar Mana.
- **On Kill Effect**:  Ganar Vida.
- **On Kill Effect**:  Explosión en base al HP enemigo.
- **Passive Effect**:  Incremento de daño.
- **Passive Effect**:  Cada vez que el objeto afectado ataca, tambien genera un ataque por la espalda.
- **Passive Effect**:  Mientras que el objeto esté equipado en en L o R Click, otorga aura.


## Extensión de Enemigos: Sistema de Drops

Se implementan tablas de drop dinámicas: Antes cada enemigo tenía hardcodeado el item que dropeaba. No se podía cambiar en runtime.

### Características
-   Es posible setear diferentes `Perfiles de Drop` a diferentes enemigos (ej: bosses dropean más legendarios y nunca comunes).
-   Dentro de un `Perfil`, es posible asignarles probabilidades a cada `Rareza`.
-  Cada `Rareza` tiene un `Pool` de posibles drops.
-   Es posible setear la condición: “El jugador como máximo verá X items de Y rareza durante la partida”.
-   Estado centralizado mediante `Singleton`. Los enemigos no necesitan hacer cálculos de probabilidades.
-  La `Rareza` de un `Item` mejora sus stats con valores parcialmente random.


## Obstáculo: Bulbo Explosivo
Se implementa un objeto que al ser golpeado y destruído explota dañando todo a su alrededor.
### Características
- Se sobreescribe el perfil de enmigo, para conservar vida, drops, etc, pero modificando su comportamiento.
- El Bulbo siempre permanece inmóvil en un punto determinado, no emite sonidos, no realiza ataques,  y no persigue al Jugador.
- Al ser destruído explota dañando al Jugador. ( Se reutiliza la explosión de la bola de fuego)
- 
## Nuevo sistema de Spawners
> [!NOTE]
> En la version anterior, un `Spawner` generaba enemigos cada x tiempo, con un máximo de enemigos configurable.
> En ciertos niveles del Jugador, se agregaban más spawners. Se observó en playtesting que los jugadores perdían el sentido de dirección al estar siempre siendo asediados por enemigos, generando una visión de túnel.
> El nuevo sistema busca dirigir mejor la atención del Jugador, y proveer momentos de descanso (para revisar items, tal vez).

### Características
- a
- b

## Enemigo: Final Boss
Después de encender las fogatas, el jugador se enfrenta con un nuevo enemigo con comportamiento más complejo
### Características
- Arena predeterminada para prevenir comportamientos inesperados de pathfinding y exploits por parte del Jugador.
- El Final Boss puede alejarse del jugador, sin estar huyendo. Algunos ataques son a distancia, otros de cercanía.
- El final boss telegrafía cada uno de sus ataques con una animación y sonido de X segundos.
- El Final Boss puede generar Bulbos Explosivos.
- El Final Boss puede cargar en linea recta.
- El FInal Boss puede disparar un proyectil múltiple.
- El Final Boss puede atacar en Melee.
- State Machine dedicado para decidir sus ataques, junto con un componente Random.
## Misceláneas: Feedback mejorado y Pasivas
- Se implementan numerosos efectos visuales y de sonido para mejorar el feedback de las funcionalidades que ya existían.
- ### Nuevas Pasivas de lvl 8 y 16
> [!NOTE]
> Como algunos efectos otorgados por pasivas se trasladaron al sistema de Slots, en su lugar ahora se agregan las siguientes mejoras
  - Summon Redesign: El summon puede llevar cualquier arma que se le asigne desde el inventario, ganando todos los bonus de ella.
  - "Tu Dash hace daño físico y tiene 50% menos de cooldown"
  - "Cuando Recolectas Esencia, explota haciendo daño a los enemigos"
  - "Por cada 1% de vida faltante, haces 0,5% más de daño"
  - "Tu Dash Genera una explosion en los puntos de inicio y fin
 ### Nuevas Armas
 - Tridente: Más lento que la lanza, pero genera un abanico de 3 líneas de AOE
 - Martillo: Súper lento, pero hace daño masivo en un círculo enfrente del portador.
