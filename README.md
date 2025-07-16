# Horde Slayer Design Document
> Autor: Santiago Cabo
> 
![Screenshot del juego donde se muestra el inventario abierto, mostrando la interface](https://storage.googleapis.com/santi-documents/ScreenJuego.png)
<h3>Este Major Update es una ampliación bastante ambiciosa del Juego. La mayor característica que se buscó es la facilidad para implementar más features mediante sistemas modulares y extendibles basados en interfaces.</h3>

## Mecánica de Personaje : Sistema de inventario

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

## Power Up: Efectos mediante Sistema de Slots

Para los Items del punto anterior, se habilita la opción de asignarle slots, donde se pueden insertar mejoras. Estas mejoras son, a su vez, otros items.
### Características
- Se puede marcar un `Item` como `Efecto Insertable`. (OnHit, OnKill, Passive)
- Se puede marcar un `Item` como `Contenedor de Slots`.  (OnHit, OnKill, Passive)
- Mediante inventario es posible arrastrar los `Efectos Insertables`.
- Si se suelta el `Insertable` sobre un `Contenedor`, el `Insertable` se elimina del inventario y el `Item Contenedor` gana un bonus.
- Si el `Item` es un `Contenedor de Slots`, su `Panel de Info` muestra los Slots.
- Si el `Item` es un `Contenedor de Slots`, por cada Efecto Insertado, se muestra un efecto visual cuando está en piso.

### Implementados
| |Tipo|Efecto|
|---|---|---|
 |<img src="https://via.placeholder.com/100" alt="Image 1" style="display:block; margin:auto;">|On Hit Effect |  Golpe crítico.|
 |<img src="https://via.placeholder.com/100" alt="Image 1" style="display:block; margin:auto;">|On Hit Effect |  Prender fuego a los enemigos.|
 |<img src="https://via.placeholder.com/100" alt="Image 1" style="display:block; margin:auto;">|On Hit Effect |  Knockback.|
 |<img src="https://via.placeholder.com/100" alt="Image 1" style="display:block; margin:auto;">|On Kill Effect |  Ganar Mana.|
 |<img src="https://via.placeholder.com/100" alt="Image 1" style="display:block; margin:auto;">|On Kill Effect |  Ganar Vida.|
 |<img src="https://via.placeholder.com/100" alt="Image 1" style="display:block; margin:auto;">|On Kill Effect |  Explosión en base al HP enemigo.|
 |<img src="https://via.placeholder.com/100" alt="Image 1" style="display:block; margin:auto;">|Passive Effect |  Incremento de daño.|
 |<img src="https://via.placeholder.com/100" alt="Image 1" style="display:block; margin:auto;">|Passive Effect |  Cada vez que el objeto afectado ataca, tambien genera un ataque por la espalda.|
 |<img src="https://via.placeholder.com/100" alt="Image 1" style="display:block; margin:auto;">|Passive Effect |  Mientras que el objeto esté equipado en en L o R Click, otorga aura.|

  
## Mecánica de Gameplay : Sistema de Drops

Se implementan tablas de drop dinámicas: Antes cada enemigo tenía hardcodeado el item que dropeaba. No se podía cambiar en runtime.

### Características
-   Es posible setear diferentes `Perfiles de Drop` a diferentes enemigos (ej: bosses dropean más legendarios y nunca comunes).
-   Dentro de un `Perfil`, es posible asignarles probabilidades a cada `Rareza`.
-  Cada `Rareza` tiene un `Pool` de posibles drops.
-   Es posible setear la condición: “El jugador como máximo verá X items de Y rareza durante la partida”.
-   Estado centralizado mediante `Singleton`. Los enemigos no necesitan hacer cálculos de probabilidades.
-  La `Rareza` de un `Item` mejora sus stats con valores parcialmente random.


## Obstáculo : Bulbo Explosivo
Se implementa un objeto que al ser golpeado y destruído explota dañando todo a su alrededor.
### Características
- Se sobreescribe el perfil de enmigo, para conservar vida, drops, etc, pero modificando su comportamiento.
- El Bulbo siempre permanece inmóvil en un punto determinado, no emite sonidos, no realiza ataques,  y no persigue al Jugador.
- Al ser destruído explota dañando al Jugador. ( Se reutiliza la explosión de la bola de fuego)
- 
## Mecánica de Gameplay : Nuevo sistema de Encounters
> [!NOTE]
> En la version anterior, un `Spawner` generaba enemigos cada x tiempo, con un máximo de enemigos configurable.
> En ciertos niveles del Jugador, se agregaban más spawners. Se observó en playtesting que los jugadores perdían el sentido de dirección al estar siempre siendo asediados por enemigos, generando una visión de túnel.
> El nuevo sistema busca dirigir mejor la atención del Jugador, y proveer momentos de descanso (para revisar items, tal vez).

### Características
- Un `Spawner` genera X de un tipo de `Enemigo`, cada Y tiempo. Cuando generó X, se destruye. 
- Cualquier `Enemigo` puede ser promovido a `Elite`, sin tener que tocar los `Spawners`.
- Se implementó el componente `Encounter`. Este componente trackea el tiempo desde que se instancia, y tiene una tabla que cada X segundos genera un `Evento`.
- Los `Eventos` que puede generar un Encounter son:
    - Spawnear un sólo enemigo de tipo X
    - Crear e iniciar Spawner de tipo X.
    - Convertir un Enemigo random en Elite.
    - Convertir un Enemigo random de tipo X en Elite.
- Cuando el `Encounter` se queda sin `Eventos` que generar, se destruye.
- Cada vez que se activa una fogata se inicia un `Encounter`.
- No es posible encender una fogata mientras exista un `Encounter` activo.
- Además de los `Encounters`, hay `Enemigos` fijos por el mapa.
- Se agrega el estado **--IDLE--** a los `Enemigos`, que hace que permanezcan en su lugar y no persigan al jugador.
- Cuando un `Enemigo` pasa mucho tiempo lejos del Jugador, se teletrasporta cerca.

## Rejugabilidad: Modo Dios y Modo infinito.
Se implementan dos modos adicionales de juego.

### Características
- En la Pantalla de Game Over, se puede reiniciar la partida en Modo Dios(todo daño recibido se reduce a cero)
- Al ganar la partida, se puede elegir entrar en Modo Infinito: 
- Oleadas infinitas de enemigos mediante un Encounter especial que dura indefinidamente.
- No se limita la cantidad de drops (Excepto por cuestiones de performance)
- Las fogatas se pueden activar varias veces y su costo aumenta exponencialmente con cada uso.
- El Modo infinito también aparecerá en el Menu.
- 
## Enemigo: Bonfire Guardian
Después de encender cada fogata, Spawnea un nuevo `Enemigo` a modo de mini-boss.
### Características
- Nuevo Set de Sprites y Voiceovers
- Ataca a distancia con un trail que trackea al Jugador.
- Cuando el trail conecta, en esa posición, luego de un segundo se genera un AOE que hace daño.
- Si el Jugador se le acerca, intentará escapar con un Dash (Se adapta método Dash del Jugador)
- El Dash de este enemigo tiene un cooldown a efectos de balance.
- Se hace más fuerte por cada fogata encendida.
  
## Enemigo: Final Boss
Después de encender todas las fogatas, se le indica al jugador que visite un area designada donde se enfrenta con un `Enemigo` con comportamiento más complejo
### Características
- Arena predeterminada para prevenir comportamientos inesperados de pathfinding y exploits por parte del Jugador.
- El Final Boss puede alejarse del jugador, sin estar huyendo. Algunos ataques son a distancia, otros de cercanía.
- El final boss telegrafía cada uno de sus ataques con una animación y sonido de X segundos.
- El Final Boss puede generar Bulbos Explosivos.
- El Final Boss puede cargar en linea recta.
- El FInal Boss puede disparar un proyectil múltiple.
- El Final Boss puede atacar en Melee.
- State Machine dedicado para decidir sus ataques, junto con un componente Random.
  
## Power Up: Nuevas Pasivas de lvl 8 y 16
> [!NOTE]
> Como algunos efectos otorgados por pasivas se trasladaron al sistema de Slots, en su lugar ahora se agregan las siguientes mejoras

| |Tipo| Descripcion |
|---|---|---|
|<img src="https://via.placeholder.com/100" alt="Image 1" style="display:block; margin:auto;"> |Redesign|El summon puede llevar cualquier arma que se le asigne desde el inventario, ganando todos los bonus de ella.|
| <img src="https://via.placeholder.com/100" alt="Image 1" style="display:block; margin:auto;">|Nuevo|Tu Dash hace daño físico y tiene 50% menos de cooldown.|
| <img src="https://via.placeholder.com/100" alt="Image 1" style="display:block; margin:auto;">|Nuevo|Cuando Recolectas Esencia, explota haciendo daño a los enemigos.|
| <img src="https://via.placeholder.com/100" alt="Image 1" style="display:block; margin:auto;">|Nuevo|Por cada 1% de vida faltante, haces 0,5% más de daño.|
| <img src="https://via.placeholder.com/100" alt="Image 1" style="display:block; margin:auto;">|Nuevo|Tu Dash Genera una explosion en los puntos de inicio y fin.|

 ### Power Up : Nuevas Armas

 | |Nombre| Descripcion |
 |---|---|---|
 | <img src="https://via.placeholder.com/100" alt="Image 1" style="display:block; margin:auto;"> |Tridente | Más lento que la lanza, pero genera un abanico de 3 líneas de AOE.|
 | <img src="https://via.placeholder.com/100" alt="Image 1" style="display:block; margin:auto;"> |Martillo | Súper lento, pero hace daño masivo en un círculo enfrente del portador.|

 ## Correcciones indicadas por docente: Quality of life.
 ### Sound Clutter
Parar no abrumar con sonidos de enemigos, se centralizan los sonidos de `Taunt` en un `Manager de Taunts`.
- El  `Manager de Taunts` tiene un `Perfil de Taunt` para cada tipo de Enemigo.
 - Los enemigos envian solicitudes de emitir sonido a `Manager de Taunts`.
 - `Manager de Taunts` elige uno de sus Audio Components y en base a parámetros configurables (max distancia, max simultaneos, etc) elige si emitir sonido o no.

  ### Claridad de Stats
  - En la pantalla de subir stats, cuando se hace Hover sobre el nombre de un stats, se iluminan los valores correspondientes que modificará.

### Relocacion de Enemigos.
Al haber un límite de enemigos, si el Jugador corría muy lejos, se quedaba sin enemigos hasta que volviera y los mate.
Para que no queden enemigos perdidos, ahora se relocalizan cada tanto.
- Se le agrega a cada enemigo un componente independiente de `Relocador`.
- El `Relocador` trackea cuanto tiempo el enemigo lleva fuera de un rango configurable.
- Si el enemigo lleva fuera de rango más que un tiempo configurable, se relocaliza.
- Al relocalizar, aparece cerca del jugador a una distancia configurable.



