# TODO LIST
## Features Parcial 2 - Feedback y Correcciones
- Sonido Dash
- Fixear Efecto cast Miasma
- Sonido de confirmación Pasivas
- Hover sobre primary stats ilumina el valor afectado.
- Remover los enemigos rezagados
- Sonido al pickear: pociones, weapons, spells, effects
- Sonido al abrirse un Hover Panel
- Sonido al abrirse un Cloeable Panel
- Sonido al apretar un Boton del Closeable Panel
- Sonido al tratar de aplicar un Effect a algo inválido.
- Sonido al consumir un Effect
- Las armas con effects tienen un particle system
- Sonido al Dropear: mismo para todos los items
- Ponerle un "Trail" a los pickables para que sea mas lindo el drop
- Passive Effect
- Passive Effect
- OnHit effect
- On Kill effect
- On Kill effect
- Duplicar el efecto de los legendarios
- Tener otra drop table para elites.

### BUGS
- Deshabilitar la E si no estoy cercca de una fogata
- Guardar upgrades si no tengo arma. Los bonus globales viven en el jugador o un dedicado
- Permitir pickear mas de 1 arma. (cada arma guarda sus propios stats, el pickup no hace swap, se agrega boton)
- No pickear items si el inventario está lleno.
- No asignar icono de arma si no la equipé

## FINAL
### Nuevo enemigo Boss
( Es enfrentado en un cuarto especial, después de prender las bonfires, para prevenir problemas de pathing)
Es rápido
Telegrafía ataques.
Tiene más de un ataque

### V2 de sistema de Oleadas.
Hay enemigos estacionarios -> Requiere nuevo estado IDLE.
Cuando se activa la fogata, después de elegir el upgrade, se inicia un countdown de 40 seg.
Al final del countdown, recién ahí queda prendida la fogata y spawnea un enemigo Boss
EL enemigo boss está garantizado de dropear un Spell.

Cuando se activan todas las fogatas, en vez de ganar, hay un voiceover y se eliminan todos los enemigos del mapa
Se dirige al jugador a un área designada del mapa.
En esta área se spawnea el "Final Boss", lockeado en IDLE
Al llegar al "centro" del area designada, a cierta distancia del boss, se deshabilitan controles, se bloquea la única salida, y se escucha un voiceover.
Se habilitan controles y el boss se vuelve hostil

### Sitema de Inventario + Sistema de Item Slots
|| Todo: Completar gráficos de Closeable Panel
|| Implementar boton AssignToRightClick
|| Implementar método AssignToTightClick en spellLauncher y Player
|| Implementar boton Equip en armas


Esc ahora sólo pausa el juego, muestra game time
Panel de Stats se invoca con "I", o clickeando una interface, se expande con inventario, formando *Panel Personaje*
Tiene Lista de *Item*, por cada uno instancia y muestra en Grid botones *ItemInventoryElement*, le setea: OnClick, On Hover, Sprite sacados de *Item*,
Panel de Personaje Captura Clicks.

- Prefab ItemInventoryElement (Button)
- Clase Abstracta: Item
       Abstractos: GetItemComponent(), GetDropComponent(), GetImplementingGameObject(), GetPanelInfo() (desc, stasts, y EffectSlot, si hay)
       OnHover: mostrar GetItemComponent()._hoveringPanel, _hoveringPanel.Open()
       OnClick: mostrar GetItemComponent()._closeablePanel 
       OnConsume: GetItemComponent()._characterPanel.Remove() (Quitar del *Panel Personaje*)

- Component ItemInventoryElement: _characterPanel, _associatedDrop, _hoveringPanel, _closeablePanel,

- Component ItemPanelBehaviour: _parent, _uiBlocker, Map<> uiGameObjects (desc, stats, slots), _isCloseable, Canvas _itemPanel.
        si _isCloseable se invoca Close() con click afuera o ESC
        Si no, se invoca Close() cuando no está activo onHover
        Open() > mostrar uiGameObjects en base a _parent.GetPanelInfo()
        Close() -> SetActive(false)

- Clase Abstracta: Drop -> OnPickup()
- Component DropBehaviour:_dropSprite,  _dropAnimation, _dropParticleSystem,   OnEnterTrigger() ->  Drop.OnPickup(), _parent, GetImplementingGameObject()

- Component: EffectSlots:
   _onHitEffect, _onKillEffects, AddOnKill, AddOnHit, IsSlotOnKillOpen(), IsSlotOnHitOpen(), TransferOnHitSlots(otherEffectSlot), _associatedItem


"Hover" Effect Slot item panel description -> "OnHit: Triggers for each enemy that takes damage from this entity. Consume an effect to fill this slot."
Penel Weapon -> botón "Drop" -> Reutiliza Swap de arma
Penel Spell ->  3 Botones -> "Reassign to X Key", no se muestra el slot donde ya está asignado.
Penel Effect -> boton "Combine" -> Abre otro panel listando los items(botones) disponlibles con slot vacío(obtener de _characterPanel) Click item-> OnConsume(): EffectSlots.AddOnKill, Super.OnConsume();

## Sistema de Drops
Enemies tienen de 0 a N components de "DropTable"
Cuando mueren pueden dropear 1 item por cada drop Table
Cada component DropTable tiene una Lista de Prefabs y de chances (o un Mapa, si se puede serializar) OnDestroy(), generan un random, selecciona el drop y lo instancian, invocando la animación de "Toss" (si es holdeable)
(en una partida normal mueren aprox 250 enemigos)
P para al menos 10 drops: 0.0554;
P para al menos 1 p≈0.0120;
Los drops pueden tener limites.
El primer elite debería aparecer antes y siempre dropear un effect.
Cada bonfire debería dropar un Spell ( asociarle una entidad invisible con "DropTanle" e invocar un destroy)


# Features utilizadas en este parcial

## Navigation Mesh
- Mirada general. Algoritmo A*
- Importar Paquete desde Github
- Nav Mesh Surface y NavMesh Layout.
- Agregar Navigation Modifiers, Y Nav Mesh Agents
- Bake.

## Animation States Básicos
- Animaciones a partir de un multi sprite, Animation Controllers
- State Machines. Editar Estados, Agregar Parámetros, Editar transiciones (in, out, duration), condiciones, sub state machines
- Animator Component

## TextMeshPro y UICanvas
- Consideraciones sobre diferentes tamaños de Pantalla.
- Rect Transform
- Anchors, Pivots

## Audio Source
- Audio Source Componnent

## Coroutines
- La necesidad de la coroutine
- IEnumerator
- Keyword yield.
- Como arrancar y parar una coroutine.

## Particle System
- Diferencias en 2d. Circulo Vs cono
- Emmision, shape, and color.

## Raycast
- La necesidad del Raycast

# Arquitectura del Software
## Estados del Jugador y los Enemigos
- Diagrama de Estados de los enemigos y el Player.

## Mecánicas
### Dash
### Ataque
### Drops de Armas
### Comprar Upgrade
### Experiencia y Level Up
### IA de los enemigos
### Spawn Random de Enemigos