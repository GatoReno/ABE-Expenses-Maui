# Project Specifications for AbeXP

## Overview
AbeXP is an income and expense tracking application built with .NET MAUI (net8.0) and Firebase. The goal is to deliver a cross-platform mobile and desktop experience that offers:

- Email and password authentication with built-in user registration.
- Recording and browsing transactions (income and expenses) with filters by date range, type, and tags.
- Financial visualizations (line, donut, bar charts) for the selected range.
- Android native widget to review recent transactions and open the quick-add form.

The solution follows an MVVM architecture backed by use cases and repositories that wrap Firebase Realtime Database.

## Target platforms
- `net8.0-android`, `net8.0-ios`, `net8.0-maccatalyst` declared in `AbeXP/AbeXP.csproj`.
- Additional support for Windows (`net8.0-windows10.0.19041.0`) when building on Windows.
- Android integrates Firebase via `Platforms/Android/google-services.json`; iOS uses `Platforms/iOS/GoogleService-Info.plist`.

## Key external dependencies
- `CommunityToolkit.Maui` and `CommunityToolkit.Mvvm`: UI components, behaviors, and MVVM source generators.
- `LiveChartsCore.SkiaSharpView.Maui`: interactive charts for the analytics view.
- `FirebaseAuthentication.net`, `FirebaseDatabase.net`, `FirebaseStorage.net`: Firebase authentication and data access.
- `FluentResults`: success/error wrapping for use case responses.
- `Controls.UserDialogs.Maui` and `CommunityToolkit.Maui.Alerts`: dialogs and toast notifications.
- `Plugin.Fingerprint_CalcDP_Maui`: prepared for biometric support (not currently wired into the flow).
- AndroidX and Material Design dependencies for Android-specific features.

## Architecture and code organization
- **MVVM**: `ViewModels/` defines state and commands; `Views/` contains the XAML pages.
- **Use cases** (`UseCases/`): encapsulate business rules (`GetTransactionsUseCase`, `CreateTransactionUseCase`, etc.) and return `Result`.
- **Services and repositories** (`Services/`, `Interfaces/`): abstract Firebase (e.g., `FibRepository<T>`, `TransactionsRepository`) and auxiliary services such as `AlertService`, `MauiNavigationService`, `SettingsService`.
- **Domain plugins** (`UseCases/Plugins/`): shared contracts such as `IUserSession`.
- **Extension methods** (`Extensions/`): mapping helpers, string localization, date formatting.
- **Shared resources** (`Resources/`): styles, colors, icons (`Resources/AppIcon`), localized strings (`Resources/Strings/AppResources.resx`).
- **Platform** (`Platforms/`): platform initialization and Android widget (`Platforms/Android/Widget`), iOS entitlements (`Platforms/iOS`).
- **Dependency injection setup**: `Extensions/ServiceCollectionExtensions.cs` registers ViewModels, Views, Firebase services, catalog lookups, and `IWidgetUpdater` per platform.

## Core functional flows
### Authentication and session
- `LoginViewModel` provides sign-in, sign-up, and simulated password recovery using `IFibAuthLog` (`FirebaseAuthService`).
- Tokens and user data are stored in `SecureStorage` and `Preferences` via `UserSession`. `App.xaml.cs` (`CheckLoginAsync`) validates the session to decide between `LoginView` and `AppShell`.

### Navigation and shell
- `AppShell` defines a `TabBar` with `MainPage` (dashboard) and `FinantialChartsPage`, plus a logout button bound to `AppShellViewModel.LogOut`.
- `MauiNavigationService` encapsulates Shell navigation (`NavigateToAsync`, `PopAsync`) and is injected into ViewModels that need it.

### Transaction management
- `MainPageViewModel` uses `IGetTransactionsUseCase` and `IDeleteTransactionUseCase` to list, filter, and delete transactions; filters honor start/end dates and type (expense/income).
- `ExpenseFormView` and `IncomeFormView` load catalogs (`GetTransactionCatalogsUseCase`) and create transactions through `ICreateTransactionUseCase`. After saving they pass a parameter to refresh the dashboard and notify the widget.

### Catalogs and localization
- Tag and payment method catalogs live in Firebase collections (`tags`, `payment_methods`) queried by `GetAllTagsUseCase` and `GetPaymentMethodsUseCase`.
- `TagModelLocalizer` and `PaymentMethodModelLocalizer` transform names using `Resources/Strings/AppResources`. The default culture is `es-MX`, configured via `SettingsService`.

### Financial charts
- `FinantialChartsViewModel` builds income/expense line, payment-type pie, and tag bar charts with LiveCharts 2.
- The selected period (three days, week, month) uses `TimePeriod` and `DateTimeExtensions.GetPeriodStart`.

### Recent transactions widget (Android)
- `QuickExpenseWidgetProvider` renders a monthly summary, remote list (`QuickExpenseListService.WidgetListService`), and button to open `AddItemActivity`.
- `WidgetListFactory` queries `IGetTransactionsUseCase` to populate up to 30 recent entries.
- `WidgetUpdater` (Android) exposes `NotifyDataChanged` and `Redraw`; it runs after signing in, creating, or deleting transactions. iOS includes a stub implementation to satisfy the contract.

### Alerts and feedback
- `AlertService` provides synchronous and asynchronous wrappers for `DisplayAlert` and toast notifications, accessible globally via `App.Alert` for errors and confirmations.

## Data modeling
- `TransactionModel` inherits from `BaseUserOwnerEntity` and generates `UserId_Date` indices for Firebase queries (`FirebaseConstants.TRANSACTIONS_COLLECTION`).
- `ExpenseTransactionModel` and `IncomeTransactionModel` enforce the `TransactionType`.
- `TransactionItem` represents UI-ready data (formatted description, localized tags, icon).
- `PaymentMethod`, `TagModel`, `TransactionCatalog` model supporting catalogs.
- `IndexItemRequest` enables paged/sorted Firebase queries via `FibRepository<T>.GetAllAsync`.

## Local persistence and configuration
- `SettingsService` wraps preferences: `FireBaseRef`, `Culture`, and `AuthAccessToken`. `FireBaseRef` defaults to `FirebaseConstants.REF`.
- `UserSession` manages tokens (`Token`, `RefreshToken`, expiration) in `SecureStorage` and basic profile data in `PreferencesConstants`.
- `App.ConfigureCulture` applies the stored culture for date and currency formatting.
- Firebase keys (`FirebaseConstants.KEY`, `PROJECTID`, etc.) are currently in source; move them to secure configuration for production.

## Resources and styles
- `App.xaml` loads `Resources/Styles/Colors.xaml` and `Resources/Styles/Styles.xaml`, defining consistent styles for MAUI controls.
- Fonts registered in `MauiProgram` (`OpenSans`, `MaterialIcons-Regular`) support icons surfaced via `Util/MaterialIconsRegular.cs`.
- `Resources/Strings/AppResources.resx` centralizes strings (Spanish by default) reused in Views, ViewModels, and the widget.

## Platform-specific elements
- **Android**: includes `MainActivity`, `MainApplication`, dialog layout (`Platforms/Android/Resources/layout/dialog_add_item.xml`), and widget XML definitions (`Resources/xml/quickexpense_widget_provider.xml`, `my_widget_provider.xml`). The manifest is generated from MAUI defaults.
- **iOS**: `AppDelegate`, entitlement configuration (`AbeXP.entitlements`), Firebase services file, and a placeholder `WidgetUpdater`.
- **MacCatalyst/Windows**: supported generically by MAUI; review Firebase permissions when enabling them.

## Build and run requirements
1. Install .NET 8 SDK and MAUI workloads: `dotnet workload install maui`.
2. Restore dependencies: `dotnet restore AbeXP.sln`.
3. Build: `dotnet build AbeXP.sln`.
4. Run on Android (example): `dotnet build AbeXP/AbeXP.csproj -t Run -f net8.0-android`.
5. Provide valid Firebase files (`google-services.json`, `GoogleService-Info.plist`).
6. Configure certificates and platform-specific permissions for device deployments.

## Testing status and observations
- No unit or UI tests are included. Use cases and services would benefit from test doubles to cover Firebase interactions.
- `WidgetListFactory.LoadData().Wait()` blocks; consider a fully async implementation to avoid deadlocks in more complex scenarios.
- Firebase configuration is tightly coupled to the current constants (`FirebaseConstants`); externalize per environment when possible.
- The biometrics package is not yet integrated; validate if it is required or remove it to reduce dependencies.

## Suggested next steps
- Implement unit tests for `UseCases` and `UserSession`.
- Externalize Firebase keys and references into secure, environment-specific configuration.
- Complete the `IWidgetUpdater` implementation on iOS or remove the conditional registration if it is not needed.
- Evaluate offline support or local caching for transactions.
- Review error handling in Firebase services (messages are currently generic).
