# Architecture Hexagonale – Application d'entreprise .NET

Schéma d'architecture **Ports & Adapters** pour une application de gestion des commandes, clients, produits et factures.

Ce projet implémente cette architecture : **Hexagonal.Domain**, **Hexagonal.Application**, **Hexagonal.Infrastructure**, **Hexagonal.API**. Les contrôleurs (Customers, Orders, Products, Invoices) sont les adaptateurs entrants ; EF Core et les repositories sont les adaptateurs sortants.

## Principe des dépendances

- **Dépendances orientées vers le centre** : les adaptateurs et ports dépendent du domaine / application, jamais l'inverse.
- **Domaine et Application** : au centre (cœur métier), sans dépendance vers l'infrastructure.

---

## Diagramme Mermaid

```mermaid
flowchart LR
    subgraph ADAPTATEURS_ENTRANTS["🖥️ ADAPTATEURS ENTRANTS"]
        A1[OrdersController<br/>API REST]
        A2[Web App / Frontend<br/>Angular ou Vue]
        A3[Message Consumer]
    end

    subgraph PORTS_ENTRANTS["📥 PORTS ENTRANTS"]
        P1[ICreateOrderInputPort]
        P2[IValidatePaymentInputPort]
        P3[IGenerateInvoiceInputPort]
    end

    subgraph HEXAGONE["⚙️ DOMAINE & APPLICATION"]
        subgraph DOMAINE["DOMAINE"]
            E1[Entité : Commande]
            E2[Entité : Client]
            E3[Entité : Facture]
            VO[Value Objects]
            DS[Services métier<br/>Règles métier]
        end
        subgraph APPLICATION["APPLICATION"]
            U1[CreateOrderUseCase]
            U2[ValidatePaymentUseCase]
            U3[GenerateInvoiceUseCase]
        end
    end

    subgraph PORTS_SORTANTS["📤 PORTS SORTANTS"]
        O1[IOrderRepository]
        O2[ICustomerRepository]
        O3[IPaymentGateway]
        O4[IInvoiceService]
        O5[IEventPublisher]
    end

    subgraph ADAPTATEURS_SORTANTS["🔌 ADAPTATEURS SORTANTS"]
        B1[SQL Server / PostgreSQL]
        B2[Stripe / Paiement]
        B3[Service Email]
        B4[RabbitMQ / Kafka]
        B5[ERP / CRM externe]
    end

    A1 --> P1
    A2 --> P2
    A3 --> P3
    P1 --> U1
    P2 --> U2
    P3 --> U3
    U1 --> O1
    U2 --> O3
    U3 --> O4
    U1 --> O5
    O1 --> B1
    O2 --> B1
    O2 --> B5
    O3 --> B2
    O4 --> B3
    O5 --> B4
```

---

## Légende

| Zone | Rôle |
|------|------|
| **Domaine** | Entités, Value Objects, services et règles métier. Aucune dépendance externe. |
| **Application** | Cas d'usage (use cases) qui orchestrent le domaine. |
| **Ports entrants** | Interfaces appelées par les adaptateurs pour déclencher les cas d'usage. |
| **Ports sortants** | Interfaces implémentées par les adaptateurs (repositories, gateways, events). |
| **Adaptateurs entrants** | Contrôleurs, UI, consumers : traduisent les requêtes en appels aux ports. |
| **Adaptateurs sortants** | Implémentations concrètes (BDD, API, email, broker). |

---
