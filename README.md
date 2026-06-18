# TruckOrder — Backend

API REST en **C# .NET 10** para TruckOrder, la app de los food trucks de Truck & Roll.

Es la continuación de los TFU anteriores:
- **UT2** definio los requerimientos y los PUC.
- **UT3** modelo el dominio (diagrama de clases con `Repository` y `Service`) y los diagramas UML.
- **UT4** diseñó la UI para PUC-001 y PUC-003.
- **UT5/UT6** (este repo) implementa el backend que el front (en outsourcing) va a consumir.

---

## Requisitos

- **.NET 10 SDK** ([descargar](https://dotnet.microsoft.com/en-us/download/dotnet/10.0))

No hace falta SQL Server: usamos **SQLite** que viene por defecto. Tampoco hay que correr migraciones a mano lo hace automaticamente: la API crea la base y carga datos de prueba al arrancar.

---

## Cómo correrlo

```powershell
cd TruckOrder.Api
dotnet restore
dotnet run
```

La API queda en **http://localhost:5000** y se abre automáticamente la documentación interactiva en `/swagger`.

---

## Estructura del proyecto

```
TruckOrder.Api/
├── Domain/
│   ├── Enums/                 # EstadoPedido, MetodoPago, EstadoPago
│   ├── Entities/              # Pedido, ItemPedido, Producto, Pago, FoodTruck, Categoria
│   └── Repositories/          # IRepoPedidos, IRepoProductos, ...  (interfaces)
├── Infrastructure/
│   ├── TruckOrderDbContext.cs # EF Core
│   ├── Seed.cs                # Carga datos de prueba en el primer arranque
│   └── Repositories/          # RepoPedidos, RepoProductos, ...    (implementaciones)
├── Application/
│   ├── PasarelaPago/          # IPasarelaPago + mock
│   ├── PaymentStrategies/     # Strategy: una clase por metodo de pago
│   └── Services/              # MenuService, PedidosService, CobrosService
├── Api/
│   ├── Controllers/           # ProductosController, PedidosController, CocinaController
│   └── Dtos/                  # DTOs de entrada y salida
├── Program.cs                 # Wiring de DI, EF y Swagger
├── appsettings.json
├── TruckOrder.Api.csproj
├── TruckOrder.Api.http        # Ejemplos para REST Client / Visual Studio
└── README.md
```

---

## Documentación de la API

Cuando levantes la API, Swagger se abre solo en http://localhost:5000/swagger. Es la herramienta principal para que el equipo de front (en outsourcing) pueda explorar y probar los endpoints sin instalar nada.

- **Swagger UI:** http://localhost:5000/swagger
- **OpenAPI JSON:** http://localhost:5000/swagger/v1/swagger.json

---

## Probar los endpoints

### Opción A — Archivo `TruckOrder.Api.http`

Si usás **Visual Studio 2022** o **VS Code con la extensión REST Client**, abrí `TruckOrder.Api.http` y hacé click arriba de cada bloque para mandar la request. Ya está armada la secuencia: crear pedido → agregar items → cobrar → cocina → entregar.

### Opción B — Swagger UI

Andá a http://localhost:5000/swagger y probá cada endpoint con el botón **"Try it out"**.

---

## Endpoints

| Método | Endpoint                                  | Descripción                                              | PUC |
| ------ | ----------------------------------------- | -------------------------------------------------------- | --- |
| GET    | `/api/trucks/{id}/productos`              | Menú del food truck (pantalla del cajero)                | 001 |
| GET    | `/api/menu-qr/{codigo}`                   | Menú público (cliente que escanea el QR)                 | 001 |
| POST   | `/api/pedidos`                            | Crear pedido nuevo                                        | 001 |
| GET    | `/api/pedidos/{id}`                       | Ver pedido                                                | 001 |
| POST   | `/api/pedidos/{id}/items`                 | Agregar ítem a la comanda                                 | 001 |
| DELETE | `/api/pedidos/{id}/items/{itemId}`        | Quitar ítem                                               | 001 |
| POST   | `/api/pedidos/{id}/cobrar`                | Cobrar (efectivo / tarjeta / MercadoPago)                 | 001 |
| POST   | `/api/pedidos/{id}/cancelar`              | Cancelar pedido                                           | 001 |
| PATCH  | `/api/pedidos/{id}/estado`                | Cambiar estado (preparación / listo / entregado)         | 003 |
| GET    | `/api/trucks/{id}/cocina/pedidos`         | Lista para la pantalla de cocina                          | 003 |
| GET    | `/api/trucks/{id}/publica/listos`         | Lista para la pantalla pública                            | 003 |

---

## Decisiones de diseño

### Arquitectura por capas

Mantenemos las capas que ya teníamos en el modelo de la UT3:

- **Domain**: entidades, enums e **interfaces** de Repository. No depende de EF Core. Las entidades tienen comportamiento (`AgregarItem`, `RecalcularTotal`, `CambiarEstado`), no son anémicas.
- **Infrastructure**: el `DbContext` y las implementaciones concretas de los repositorios.
- **Application**: servicios que orquestan operaciones del dominio y las estrategias de cobro.
- **Api**: controladores, DTOs y wiring de DI.

### Principios SOLID

- **S — Single Responsibility:** cada servicio tiene un motivo único de cambio (`MenuService`, `PedidosService`, `CobrosService`). Los controllers no contienen lógica de negocio.
- **O — Open/Closed:** agregar un nuevo método de pago (por ejemplo, transferencia bancaria) solo requiere crear una nueva `IEstrategiaCobro`. `CobrosService` no se toca.
- **L — Liskov:** cualquier `IPasarelaPago` se puede reemplazar (mock para dev, MP real para producción) sin que el resto se entere.
- **I — Interface Segregation:** los repositorios son chicos y enfocados (`IRepoPedidos`, `IRepoPagos`, ...). No hay un "repositorio genérico" lleno de métodos.
- **D — Dependency Inversion:** los servicios dependen de las **interfaces** del Domain. La inyección concreta vive en `Program.cs` con el contenedor de DI nativo de .NET.

### Patrones aplicados

- **Repository** abstrae el acceso a datos. Las implementaciones EF Core se pueden cambiar (por ejemplo, por mocks para tests) sin tocar los servicios.
- **Service (DDD)** — `PedidosService`, `CobrosService`, `MenuService` orquestan operaciones que no son responsabilidad natural de una sola entidad.
- **Strategy** — los tres métodos de pago se implementan como estrategias intercambiables en `Application/PaymentStrategies`. Esto es lo que hace fácil cumplir Open/Closed.
- **DTO** — las clases en `Api/Dtos` separan el contrato externo de la API del modelo de dominio interno.
- **Dependency Injection** — el contenedor nativo de .NET (`Microsoft.Extensions.DependencyInjection`) hace todo el wiring desde `Program.cs`.

### Cambios respecto al modelo de la UT3

- En la UT3 teníamos `ServicioNotificaciones` y `ServicioReportes`. Por alcance los dejamos para una próxima iteración: la cocina y la pantalla pública leen el estado del pedido directamente con sus endpoints, sin push.
- El campo `Numero` del pedido se autoincrementa por food truck. El repositorio expone `ProximoNumeroAsync(truckId)`.
- Agregamos `MontoRecibido` y `Cambio` a `Pago` para soportar el flujo de efectivo (alineado con la UI del PUC-001 de UT4).

### Pendientes para una próxima vuelta

- **Autenticación.** Hoy la API es abierta. En producción agregaríamos JWT con roles (cajero, cocinero, administrador).
- **Webhook de Mercado Pago.** La `PasarelaPagoMock` resuelve sincrónicamente. Para MP real habría que sumar un endpoint que reciba la confirmación asíncrona.
- **Migraciones EF Core.** Usamos `EnsureCreated()`. Para producción meteríamos `Add-Migration` / `Update-Database`.
- **Tests.** La arquitectura ya está preparada (los servicios reciben interfaces de Repository), pero la suite queda para otra vuelta.

---

## Repositorio

Una vez clonado:

```powershell
git clone https://github.com/valentinhernandez1/truckorder-backend
cd truckorder-backend\TruckOrder.Api
dotnet restore
dotnet run
```

Y después http://localhost:5000/swagger.
