# Requirements Document

## Introduction

This feature completes the BytesRewards Angular 20 frontend by implementing four missing pages: the Employee Wallet page, the Employee Rewards page, the Leaderboard page, and the Manager Rewards History page. Each page communicates with the existing .NET backend at `http://localhost:7000`, uses Keycloak JWT authentication, follows the established standalone-component/signals pattern, and is styled with Tailwind CSS.

## Glossary

- **Application**: The BytesRewards Angular 20 single-page application.
- **Employee**: A user with the `employee` Keycloak role.
- **Manager**: A user with the `manager` Keycloak role.
- **Auth_Service**: The Angular `AuthService` that wraps Keycloak and exposes the current user's identity.
- **Wallet_Service**: The new Angular service responsible for calling `GET /wallets/{userId}` and `GET /wallets/ledger/{userId}`.
- **Wallet_Page**: The Angular standalone component rendered at `/wallet`.
- **Wallet**: The backend resource representing an employee's bytes balance (`availableBytes`).
- **Ledger_Entry**: A single transaction record returned by `GET /wallets/ledger/{userId}` with fields `rewardId`, `rewardCategoryName`, `bytes`, `awardedBy`, `reason`, and `awardedAt`.
- **Employee_Rewards_Page**: The Angular standalone component rendered at `/employee/rewards`.
- **Reward_History_Entry**: A record returned by `GET /rewards/history/{userId}` with fields `rewardId`, `rewardCategoryName`, `bytes`, `awardedBy`, `reason`, and `awardedAt`.
- **Leaderboard_Page**: The Angular standalone component rendered at `/leaderboard`.
- **Leaderboard_Entry**: A record returned by `GET /leaderboard` with fields `rank`, `userId`, `employeeName`, and `totalEarnedBytes`.
- **Manager_Rewards_History_Page**: The Angular standalone component rendered at `/manager/rewards-history`.
- **Manager_Reward_Entry**: A record returned by `GET /rewards` with fields `id`, `fromUserName`, `toUserName`, `rewardCategoryName`, `bytes`, `reason`, and `createdAt`.
- **Sidebar**: The shared `SidebarComponent` present on every page.
- **Topbar**: The shared `TopbarComponent` present on every page.

---

## Requirements

### Requirement 1: Auth Service User ID

**User Story:** As a developer, I want to retrieve the current user's Keycloak subject ID from the Auth Service, so that frontend services can pass it to user-scoped backend endpoints.

#### Acceptance Criteria

1. THE Auth_Service SHALL expose a `getUserId()` method that returns the `sub` claim from the Keycloak token as a `string`.
2. WHEN the Keycloak token is absent or not yet parsed, THE Auth_Service SHALL return an empty string from `getUserId()`.

---

### Requirement 2: Wallet Service

**User Story:** As a developer, I want a dedicated Angular service that fetches wallet data, so that the Wallet_Page can display balance and transaction history without embedding HTTP logic in the component.

#### Acceptance Criteria

1. THE Wallet_Service SHALL expose a `getWallet(userId: string)` method that calls `GET /wallets/{userId}` and returns an `Observable<{ availableBytes: number }>`.
2. THE Wallet_Service SHALL expose a `getWalletLedger(userId: string)` method that calls `GET /wallets/ledger/{userId}` and returns an `Observable<LedgerEntry[]>`.
3. THE Wallet_Service SHALL be provided in the Angular root injector so all components can inject it without additional configuration.

---

### Requirement 3: Wallet Page

**User Story:** As an Employee, I want to see my current bytes balance and a full transaction history on the Wallet page, so that I can track how I have earned bytes over time.

#### Acceptance Criteria

1. WHEN the Wallet_Page initialises, THE Application SHALL call `GET /wallets/{userId}` with the current user's ID and display the returned `availableBytes` balance prominently.
2. WHEN the Wallet_Page initialises, THE Application SHALL call `GET /wallets/ledger/{userId}` with the current user's ID and display the returned ledger entries in a list or table ordered by `awardedAt` descending.
3. WHILE data is loading, THE Wallet_Page SHALL display a loading indicator in place of the balance and ledger.
4. IF the wallet request fails, THEN THE Wallet_Page SHALL display an inline error message stating that the balance could not be loaded.
5. IF the ledger request fails, THEN THE Wallet_Page SHALL display an inline error message stating that the transaction history could not be loaded.
6. WHEN the ledger is empty, THE Wallet_Page SHALL display an empty-state message indicating no transactions have been recorded.
7. THE Wallet_Page SHALL include the Sidebar with `workspaceTitle="Employee Workspace"` and the employee navigation menu, and the Topbar.
8. THE Wallet_Page SHALL be accessible only to authenticated users via the existing `authGuard`.
9. THE Wallet_Page SHALL be reachable at route `/wallet` and registered in `app.routes.ts` as a lazy-loaded component.

---

### Requirement 4: Employee Rewards Page

**User Story:** As an Employee, I want to see all rewards I have received on a dedicated Rewards page, so that I can review my recognition history.

#### Acceptance Criteria

1. WHEN the Employee_Rewards_Page initialises, THE Application SHALL call `GET /rewards/history/{userId}` with the current user's ID and display each returned `Reward_History_Entry`.
2. WHEN the rewards history is non-empty, THE Employee_Rewards_Page SHALL display, for each entry: `rewardCategoryName`, `bytes`, `awardedBy`, `reason`, and `awardedAt`.
3. WHILE data is loading, THE Employee_Rewards_Page SHALL display a loading indicator.
4. IF the rewards history request fails, THEN THE Employee_Rewards_Page SHALL display an inline error message.
5. WHEN the rewards history is empty, THE Employee_Rewards_Page SHALL display an empty-state message indicating no rewards have been received.
6. THE Employee_Rewards_Page SHALL include the Sidebar with `workspaceTitle="Employee Workspace"` and the employee navigation menu, and the Topbar.
7. THE Employee_Rewards_Page SHALL be accessible only to authenticated users via the existing `authGuard`.
8. THE Employee_Rewards_Page SHALL be reachable at route `/employee/rewards` and registered in `app.routes.ts` as a lazy-loaded component.
9. THE Application SHALL update the `employee-menu.ts` entry for "Rewards" to point to `/employee/rewards`.

---

### Requirement 5: Leaderboard Page

**User Story:** As an Employee, Manager, or Admin, I want to see a ranked leaderboard of employees by total earned bytes, so that I can understand team performance and celebrate top performers.

#### Acceptance Criteria

1. WHEN the Leaderboard_Page initialises, THE Application SHALL call `GET /leaderboard` and display the returned `Leaderboard_Entry` list ordered by `rank` ascending.
2. WHEN the leaderboard is non-empty, THE Leaderboard_Page SHALL display, for each entry: `rank`, `employeeName`, and `totalEarnedBytes`.
3. WHILE data is loading, THE Leaderboard_Page SHALL display a loading indicator.
4. IF the leaderboard request fails, THEN THE Leaderboard_Page SHALL set its page state to ERROR and display an inline error message.
5. WHEN the leaderboard response is empty after a successful request, THE Leaderboard_Page SHALL set its page state to EMPTY and display an empty-state message.
6. THE Leaderboard_Page SHALL visually distinguish the top three ranked entries (rank 1, 2, 3) from the rest.
7. THE Leaderboard_Page SHALL include the Sidebar with `workspaceTitle="Employee Workspace"` and the employee navigation menu, and the Topbar.
8. THE Leaderboard_Page SHALL be accessible only to authenticated users via the existing `authGuard`.
9. THE Leaderboard_Page SHALL be reachable at route `/leaderboard` and registered in `app.routes.ts` as a lazy-loaded component.

---

### Requirement 6: Manager Rewards History Page

**User Story:** As a Manager, I want to see all rewards I have assigned on a dedicated history page, so that I can track my recognition activity and see when and to whom I awarded bytes.

#### Acceptance Criteria

1. WHEN the Manager_Rewards_History_Page initialises, THE Application SHALL call `GET /rewards` and display the returned `Manager_Reward_Entry` list ordered by `createdAt` descending.
2. WHEN the rewards list is non-empty, THE Manager_Rewards_History_Page SHALL display, for each entry: `toUserName`, `rewardCategoryName`, `bytes`, `reason`, and `createdAt`.
3. WHILE data is loading, THE Manager_Rewards_History_Page SHALL display a loading indicator.
4. IF the rewards request fails, THEN THE Manager_Rewards_History_Page SHALL display an inline error message and SHALL NOT display the empty-state message.
5. WHEN the rewards list is empty after a successful request, THE Manager_Rewards_History_Page SHALL display an empty-state message indicating no rewards have been assigned.
6. THE Manager_Rewards_History_Page SHALL include the Sidebar with `workspaceTitle="Manager Workspace"` and the manager navigation menu, and the Topbar.
7. THE Manager_Rewards_History_Page SHALL be accessible only to users with the `manager` role via the existing `managerGuard`.
8. THE Manager_Rewards_History_Page SHALL be reachable at route `/manager/rewards-history` and registered in `app.routes.ts` as a lazy-loaded component.
9. THE Application SHALL update the `manager-menu.ts` entry for "Rewards" to point to `/manager/rewards-history`.

---

### Requirement 7: Leaderboard Service

**User Story:** As a developer, I want a dedicated Angular service that fetches leaderboard data, so that the Leaderboard_Page has a clean separation of HTTP concerns.

#### Acceptance Criteria

1. THE Application SHALL provide a `LeaderboardService` with a `getLeaderboard()` method that calls `GET /leaderboard` and returns an `Observable<LeaderboardEntry[]>`.
2. THE `LeaderboardService` SHALL be provided in the Angular root injector.
