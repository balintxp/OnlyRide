# OnlyRide

Az **OnlyRide** egy egyetemi projekttárgy keretében (Rendszerfejlesztés és IT projektmenedzsment, 2026) készült rendszer, melynek célja egy autószerviz/műhely számára webshop és szervízidőpont-foglaló megoldás kialakítása, DotNetNuke (DNN) és HotCakes alapokon, Azure infrastruktúrán üzemeltetve.

## Technológiák

- DotNetNuke (DNN) – portál keretrendszer
- HotCakes – webshop modul
- C#, .NET
- SQL Server
- Azure (Windows szerver VM, IIS, SQL szerver)
- WinForms
- Mantis (ticketing rendszer)

## Branch-ek

### `feature/HotcakesPriceManager`
WinForms kliens alkalmazás a HotCakes webshop termékárainak kezeléséhez. Az alkalmazás lehetővé teszi az árak gyors, automatizált frissítését az adatbázison keresztül, megkönnyítve a webshop karbantartását.

### `feature/servicebooking`
Saját fejlesztésű DNN modul, amely szervízidőpont-foglalási funkciót valósít meg. A modul a webshopba integrálva teszi lehetővé az ügyfelek számára, hogy online foglaljanak időpontot szervizelésre, csökkentve a manuális, telefonos időpontfoglalás terhét.

## Infrastruktúra

A rendszer egy Azure-on futó Windows szerveren (VM) üzemel, ahol az IIS webszerver hosztolja a DNN portált és a HotCakes webshopot, az adatokat pedig egy SQL szerver tárolja. A fejlesztés és a hibakövetés Github és Mantis segítségével történt.
