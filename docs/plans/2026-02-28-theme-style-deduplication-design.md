# Theme Style Deduplication Design

**Date:** 2026-02-28
**Branch:** 46-implement-multiselection-in-products-list
**Approved approach:** C (full cleanup)

---

## Problem

Four theme files each define their own version of the same XAML styles, causing maintenance overhead:

| Duplication | Scope |
|---|---|
| `GruvboxDarkBrowseButtonStyle` / `AyuDarkBrowseButtonStyle` | Identical to each other and structurally identical to `BrowseButtonStyle` in `Generic.xaml` |
| `GruvboxDarkDataBrowserTextBoxStyle` / `AyuDarkDataBrowserTextBoxStyle` | Identical to each other |
| `BlueButtonStyle` | Misleading name for a semantic "primary action" button; defined in all 4 themes |
| 7 named styles in GruvboxDark / AyuDark | `BaseButtonStyle`, `BlueButtonStyle`, `GrayButtonStyle`, `ClearButtonStyle`, `DataGridColumnHeaderStyle`, `DataGridCellStyle`, `DataGridRowStyle` — 100% identical between the two dark themes |
| `DataBrowserBox` override template in dark themes | Uses an inferior `DataTrigger` on `SelectedItem` to control clear-button visibility; `Generic.xaml` uses the more reliable `HasSelection` `ControlTemplate.Trigger` |
| Inline anonymous clear-button style | Repeated inside the `DataBrowserBox` override template in both dark themes; a named `ClearButtonStyle` already exists |

---

## Solution

### 1. New file: `TelAvivMuni-Exercise.Themes/Themes/Dark.Styles.xaml`

Contains the 7 named styles shared by all dark themes. This file is a **canonical template only** and is **not** loaded at runtime via `MergedDictionaries`. Both `GruvboxDark.Styles.xaml` and `AyuDark.Styles.xaml` instead **inline** these 7 styles directly after their own `Colors.xaml` merge, so that `StaticResource` brush keys resolve from the theme's local colors dictionary. (WPF's `StaticResource` cannot traverse up to sibling merged dictionaries across cross-assembly pack URIs, so merging a shared file at runtime would cause resource-not-found failures.)

Styles included:
- `BaseButtonStyle`
- `PrimaryButtonStyle` *(renamed from `BlueButtonStyle`)*
- `GrayButtonStyle`
- `ClearButtonStyle`
- `DataGridColumnHeaderStyle`
- `DataGridCellStyle`
- `DataGridRowStyle`

### 2. Rename theme-prefixed DataBrowserBox styles

In each dark theme's Styles.xaml:
- `GruvboxDarkBrowseButtonStyle` → `BrowseButtonStyle`
- `AyuDarkBrowseButtonStyle` → `BrowseButtonStyle`
- `GruvboxDarkDataBrowserTextBoxStyle` → `DataBrowserTextBoxStyle`
- `AyuDarkDataBrowserTextBoxStyle` → `DataBrowserTextBoxStyle`

The `DataBrowserBox` implicit style references are updated to match.

### 3. Rename `BlueButtonStyle` → `PrimaryButtonStyle` everywhere

Files affected:
- `Blue.Styles.xaml`
- `Emerald.Styles.xaml`
- `AyuDark.Styles.xaml` (via inlined styles after refactor)
- `GruvboxDark.Styles.xaml` (via inlined styles after refactor)
- `DataBrowserDialog.xaml` (the one consumer of this style key)

### 4. Fix `DataBrowserBox` override template in dark themes (Approach C)

Replace the `DataTrigger`-based clear-button visibility with the same `HasSelection ControlTemplate.Trigger` approach used in `Generic.xaml`. Replace the inline anonymous clear-button template with a reference to the named `ClearButtonStyle`.

---

## Files Changed

| File | Change |
|---|---|
| `TelAvivMuni-Exercise.Themes/Themes/Dark.Styles.xaml` | **New** — canonical template for shared dark-theme named styles (not loaded at runtime) |
| `TelAvivMuni-Exercise.Themes.Zed.GruvboxDark/Themes/GruvboxDark.Styles.xaml` | Inline 7 shared dark-theme named styles (sourced from Dark.Styles.xaml template), rename BrowseButtonStyle and DataBrowserTextBoxStyle, fix DataBrowserBox template |
| `TelAvivMuni-Exercise.Themes.Zed.AyuDark/Themes/AyuDark.Styles.xaml` | Same as above |
| `TelAvivMuni-Exercise.Themes.Blue/Themes/Blue.Styles.xaml` | Rename `BlueButtonStyle` → `PrimaryButtonStyle` |
| `TelAvivMuni-Exercise.Themes.Emerald/Themes/Emerald.Styles.xaml` | Rename `BlueButtonStyle` → `PrimaryButtonStyle` |
| `TelAvivMuni-Exercise.Controls/DataBrowserDialog.xaml` | Update `BlueButtonStyle` reference → `PrimaryButtonStyle` |

---

## Invariants

- No visual change to any theme — all brush references remain the same semantic keys
- No runtime behavior change — `HasSelection` trigger was already correct in `Generic.xaml`; dark themes are brought to the same level
- `Generic.xaml` is not touched
- `Shared.xaml` is not touched
- Light themes (Blue, Emerald) keep their own `BaseButtonStyle`/`GrayButtonStyle` etc. (they intentionally differ from dark themes)
