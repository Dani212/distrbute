# Coding Tenets

Our principles for writing clean, maintainable, and predictable code.

---

## 📝 Quick Glance
  
1. **No hardcoding / magic strings** – use constants, config, or env vars.
2. **Reuse logic** – implement once, reuse everywhere.
3. **One source of truth** – never duplicate the same data.
4. **Descriptive names** – code should explain itself.
5. **Break down complexity** – extract conditions into variables.
6. **Be consistent** – stick to one style.
7. **Code tells a story** – minimal comments needed.
8. **Support debugging** – store results before returning.

---

## 📖 Expanded Details

### 1. No Hardcoding / No Magic Strings
- Avoid hardcoding values (e.g., URLs, tokens, IDs).
- Use constants, configuration files, or environment variables.
- Ensures flexibility and avoids brittle code.

---

### 2. Reuse Logic and Screens Wherever Possible
- Think in terms of SDKs or reusable modules.
- If you’ve solved a problem once, don’t reimplement it again.
- Strive for shared implementations to reduce duplication and inconsistencies.

---

### 3. One Source of Truth
- Any given piece of data (e.g., `CustomerName`) should exist in only one place.
- Prevents mismatches and bugs caused by duplicate storage.
- Encourages cleaner data flow.

---

### 4. Descriptive Naming
- Variables, methods, and classes should clearly describe intent.
- Avoid abbreviations unless universally understood.
- Good names reduce the need for extra comments.

---

### 5. Break Down Complex Logic with Variables
- Avoid unreadable inline conditions.  
  Example:
  ```csharp
  // Bad
  if (user.Something == someOtherThing && user.SomeOtherThing == thatOtherThing && anotherThing) { ... }

  // Good
  bool hasPermission = user.HasPermission(...);
  bool isActive = user.IsActive;
  if (hasPermission && isActive) { ... }
    ```

### 6. Consistency in Coding Patterns
- Stick to a single style (indentation, casing, naming conventions, error handling).
- Predictability in code structure reduces cognitive load.
- Makes the codebase easier for the whole team to maintain.

---

### 7. Code Should Be Self-Explanatory
- Write code so clear that it rarely needs comments.
- Code should **tell a story** through naming and structure.
- Comments should be reserved for explaining *why*, not *what*.

---

### 8. Support Debugging by Design
- Avoid one-liner returns like:
  ```csharp
  return SomeFunction();
  
  // prefer
  var result = SomeFunction();
  return result;
  ```
- This allows logging or inspecting values before returning, making issues easier to trace.
