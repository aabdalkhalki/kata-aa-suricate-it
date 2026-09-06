# Bibliothèque municipale, gestion des emprunts

API .NET 10 pour enregistrer des ouvrages et des adhérents, gérer les emprunts, les retours et les pénalités de retard. Le domaine est testé sans infrastructure, l'API est testée de bout en bout sur une base SQLite en mémoire.

## Lancer

```
dotnet test
dotnet run --project src/Library.Api
```

L'API écoute sur http://localhost:5080. Le document OpenAPI est servi sur `/openapi/v1.json` et une interface sur `/scalar` en environnement Development. Le scénario complet est rejouable depuis `src/Library.Api/Library.Api.http` (VS Code REST Client, Rider, Visual Studio). La base `library.db` est créée et migrée au démarrage.

## Règles implémentées

| Règle | Code | Test |
|---|---|---|
| Un ouvrage a un titre, un auteur et des exemplaires | `Book` | `BookTests` |
| Adhérent standard : 3 emprunts simultanés, 3 semaines | `LendingPolicy` | `Lending_policy_follows_membership_type` |
| Adhérent étudiant : 5 emprunts simultanés, 4 semaines | `LendingPolicy` | `Lending_policy_follows_membership_type` |
| Pas d'emprunt sans exemplaire disponible | `Book.LendCopy` | `Borrowing_when_no_copy_is_available_fails_with_NoCopyAvailable` |
| Pas d'emprunt au-delà du quota | `Member.Borrow` | `Member_cannot_exceed_the_simultaneous_loan_quota`, `Loan_quota_is_enforced_across_requests` |
| Retard compté en jours à partir de l'échéance | `Loan.DaysLate` | `Days_late_are_counted_from_the_due_date` |
| 0,20 € par jour de retard, plafonné à 10 € | `LateFee` | `Late_fee_is_twenty_cents_per_day_capped_at_ten_euros` |
| Total des pénalités en cours par adhérent | `Member.OutstandingPenalties` | `Outstanding_penalties_sum_every_late_return`, `Returning_nine_days_late_costs_one_euro_eighty` |

## Hypothèses

- Un adhérent ne peut pas emprunter deux exemplaires du même ouvrage en même temps ; un ouvrage enregistré deux fois compte comme deux ouvrages distincts.
- Le retard se compte en jours calendaires entiers ; rendre le jour de l'échéance n'est pas un retard.
- Le plafond de 10 € s'applique par retour, pas par adhérent.
- Les pénalités s'accumulent ; leur règlement n'est pas dans le périmètre.
- Une pénalité naît au moment du retour ; un prêt en retard non encore rendu n'en génère pas.
- Une pénalité impayée ne bloque pas un nouvel emprunt.
- Les dates sont en UTC ; une bibliothèque réelle utiliserait son fuseau.
- Les exemplaires sont un compteur sur l'ouvrage, pas des entités individuelles.

## Décisions

- Quatre projets, dépendances vers l'intérieur : `Domain` ne référence rien, `Application` référence `Domain`, `Infrastructure` référence `Application` ; `Api` référence `Application` et `Infrastructure`, c'est la racine de composition.
- Les échecs métier attendus sont des `Result` portant une erreur typée ; les violations de contrat sont des exceptions. Jamais les deux pour un même cas.
- La date du jour vient de `TimeProvider`, ce qui permet aux tests d'API d'avancer le calendrier sans attendre.
- La pénalité est calculée et stockée au moment du retour, avec le tarif en vigueur ce jour-là.
- `Book` et `Member` sont deux agrégats ; un emprunt les modifie tous les deux dans une même transaction EF Core.
- SQLite avec migrations, pour que le projet se lance sans rien installer. Le `decimal` y est stocké en texte, aucun calcul monétaire n'est fait en SQL.
- Les erreurs HTTP sont des `ProblemDetails` (400, 404, 409, 500) ; les erreurs métier (404, 409) portent en plus un code dans l'extension `code`.
- Pas de MediatR ni d'AutoMapper : sept handlers et trois mappings s'écrivent à la main.

## Volontairement hors périmètre

- Concurrence sur le dernier exemplaire : une colonne `Version` en token de concurrence sur `Books`, et `DbUpdateConcurrencyException` mappée en 409.
- Règlement des pénalités.
- Exemplaires physiques identifiés individuellement.
- Réservation, prolongation, authentification.
- Test d'architecture garantissant que `Catalogue` ne dépend jamais de `Lending`.
- PostgreSQL à la place de SQLite.
