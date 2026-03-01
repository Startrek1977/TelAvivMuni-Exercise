# Theme Style Deduplication Implementation Plan

**Goal:** Eliminate cross-theme XAML style duplication by renaming theme-prefixed helper styles, extracting shared dark-theme named styles into a single file, renaming `BlueButtonStyle` → `PrimaryButtonStyle` everywhere, and fixing the dark-theme `DataBrowserBox` template to use the same reliable `HasSelection` trigger approach already used in `Generic.xaml`.

**Architecture:**
- `Dark.Styles.xaml` in `TelAvivMuni-Exercise.Themes/Themes/` is a **canonical template — not loaded at runtime**. It holds the 7 named styles shared by all dark themes as a single source-of-truth reference. It is not referenced via `MergedDictionaries` by any theme.
- **Why not merged:** WPF's `StaticResource` cannot traverse up to sibling merged dictionaries when a `ResourceDictionary` is loaded via a cross-assembly pack URI. Styles must be inlined in each theme's own `Styles.xaml` so their brush keys resolve from the local `Colors.xaml` (e.g. `GruvboxDark.Colors.xaml`).
- Each dark theme's `Styles.xaml` **inlines** all 7 shared styles and additionally defines theme-specific content: `BrowseButtonStyle`, `DataBrowserTextBoxStyle`, and the `DataBrowserBox` implicit style.
- The `DataBrowserBox` implicit style in dark themes is updated to use `Style="{StaticResource ClearButtonStyle}"` and a `ControlTemplate.Trigger` on `HasSelection`, matching `Generic.xaml`'s more reliable approach.

**Tech Stack:** WPF XAML, ResourceDictionary, `pack://application:,,,/` URIs, .NET 8.0-windows

---

## Context

### File map

| File | Role |
|---|---|
| `TelAvivMuni-Exercise.Themes/Themes/Shared.xaml` | Brush defaults for `Generic.xaml` (neutral/light fallbacks). **Do not touch.** |
| `TelAvivMuni-Exercise.Themes/Themes/Dark.Styles.xaml` | Canonical template (not loaded at runtime) — shared dark named styles inlined into each dark theme |
| `TelAvivMuni-Exercise.Themes.Zed.GruvboxDark/Themes/GruvboxDark.Styles.xaml` | Dark theme styles. Heavy changes. |
| `TelAvivMuni-Exercise.Themes.Zed.AyuDark/Themes/AyuDark.Styles.xaml` | Dark theme styles. Same changes as Gruvbox. |
| `TelAvivMuni-Exercise.Themes.Blue/Themes/Blue.Styles.xaml` | Light theme. Only rename `BlueButtonStyle` → `PrimaryButtonStyle`. |
| `TelAvivMuni-Exercise.Themes.Emerald/Themes/Emerald.Styles.xaml` | Light theme. Only rename `BlueButtonStyle` → `PrimaryButtonStyle`. |
| `TelAvivMuni-Exercise.Controls/DataBrowserDialog.xaml` | Single consumer of `BlueButtonStyle`. Rename reference. |
| `TelAvivMuni-Exercise.Controls/Themes/Generic.xaml` | Default control template. **Do not touch.** |

### Why dark themes duplicate `DataBrowserBox`

`Generic.xaml` is a `ResourceDictionary` root — it cannot reach `Application.Current.Resources`. It loads its own brush copies from `Shared.xaml`. When a dark theme is active, its app-level implicit style `TargetType="{x:Type controls:DataBrowserBox}"` overrides `Generic.xaml`'s style so the control gets dark brushes.

### Why `BrowseButtonStyle` / `DataBrowserTextBoxStyle` can be renamed

The dark themes currently use prefixed names (`GruvboxDarkBrowseButtonStyle`, etc.) to avoid perceived key collisions. But each dark theme's `Styles.xaml` is a self-contained `ResourceDictionary` merged into `Application.Current.Resources` — short, unprefixed key names like `BrowseButtonStyle` are fine as long as they're defined within that dictionary. No collision with `Generic.xaml`'s own `BrowseButtonStyle` occurs because the `DataBrowserBox` implicit style (in the app-level dict) resolves `{StaticResource BrowseButtonStyle}` from its own merged dict, not from `Generic.xaml`.

### Why `HasSelection` is better than `DataTrigger` on `SelectedItem`

The comment in `Generic.xaml:150-158` explains: a `ControlTemplate.Trigger` on a dependency property tracks change-notifications through the property engine. A `DataTrigger` binding on `SelectedItem` inside an inline `Button.Style` can silently miss DP updates after initial template application.

---

## Task 1: Create `Dark.Styles.xaml`

**Files:**
- Create: `TelAvivMuni-Exercise.Themes/Themes/Dark.Styles.xaml`

No `.csproj` edit needed — `UseWPF=true` SDK projects auto-include all `.xaml` files as `Page` items.

**Step 1: Create the file with the shared dark-theme named styles**

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <!--  ============================================  -->
    <!--  Shared Dark Theme Named Styles               -->
    <!--  Consumed by GruvboxDark, AyuDark, and any   -->
    <!--  future dark themes. Brushes are resolved     -->
    <!--  from the theme's own Colors.xaml via         -->
    <!--  Application.Current.Resources.               -->
    <!--  ============================================  -->

    <!--  Base Button Style  -->
    <Style x:Key="BaseButtonStyle" TargetType="Button">
        <Setter Property="BorderThickness" Value="0" />
        <Setter Property="FontWeight" Value="Medium" />
        <Setter Property="Cursor" Value="Hand" />
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="Button">
                    <Border Padding="16 6"
                            Background="{TemplateBinding Background}"
                            CornerRadius="4">
                        <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center" />
                    </Border>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

    <!--  Primary Button Style (Primary actions)  -->
    <Style x:Key="PrimaryButtonStyle"
           BasedOn="{StaticResource BaseButtonStyle}"
           TargetType="Button">
        <Setter Property="Background" Value="{StaticResource PrimaryBrush}" />
        <Setter Property="Foreground" Value="{StaticResource TextPrimaryBrush}" />
        <Style.Triggers>
            <Trigger Property="IsMouseOver" Value="True">
                <Setter Property="Background" Value="{StaticResource PrimaryDarkBrush}" />
            </Trigger>
            <Trigger Property="IsPressed" Value="True">
                <Setter Property="Background" Value="{StaticResource PrimaryDarkerBrush}" />
            </Trigger>
            <Trigger Property="IsEnabled" Value="False">
                <Setter Property="Background" Value="{StaticResource DisabledBrush}" />
                <Setter Property="Foreground" Value="{StaticResource TextDisabledBrush}" />
            </Trigger>
        </Style.Triggers>
    </Style>

    <!--  Gray Button Style (Secondary actions)  -->
    <Style x:Key="GrayButtonStyle"
           BasedOn="{StaticResource BaseButtonStyle}"
           TargetType="Button">
        <Setter Property="Background" Value="{StaticResource SecondaryBrush}" />
        <Setter Property="Foreground" Value="{StaticResource TextPrimaryBrush}" />
        <Style.Triggers>
            <Trigger Property="IsMouseOver" Value="True">
                <Setter Property="Background" Value="{StaticResource SecondaryDarkBrush}" />
            </Trigger>
            <Trigger Property="IsPressed" Value="True">
                <Setter Property="Background" Value="{StaticResource SecondaryDarkerBrush}" />
            </Trigger>
        </Style.Triggers>
    </Style>

    <!--  Clear Button Style (Small circular button with X)  -->
    <Style x:Key="ClearButtonStyle" TargetType="Button">
        <Setter Property="Width" Value="20" />
        <Setter Property="Height" Value="20" />
        <Setter Property="Background" Value="Transparent" />
        <Setter Property="BorderThickness" Value="0" />
        <Setter Property="Content" Value="&#x2715;" />
        <Setter Property="Cursor" Value="Hand" />
        <Setter Property="FontSize" Value="12" />
        <Setter Property="Foreground" Value="{StaticResource TextDisabledBrush}" />
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="Button">
                    <Border Background="{TemplateBinding Background}" CornerRadius="10">
                        <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center" />
                    </Border>
                    <ControlTemplate.Triggers>
                        <Trigger Property="IsMouseOver" Value="True">
                            <Setter Property="Background" Value="{StaticResource NeutralHoverBrush}" />
                            <Setter Property="Foreground" Value="{StaticResource TextSecondaryBrush}" />
                        </Trigger>
                    </ControlTemplate.Triggers>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

    <!--  DataGrid Column Header Style  -->
    <Style x:Key="DataGridColumnHeaderStyle" TargetType="DataGridColumnHeader">
        <Setter Property="Background" Value="{StaticResource NeutralExtraLightBrush}" />
        <Setter Property="Foreground" Value="{StaticResource TextSecondaryBrush}" />
        <Setter Property="FontWeight" Value="SemiBold" />
        <Setter Property="FontSize" Value="12" />
        <Setter Property="Padding" Value="12 10" />
        <Setter Property="BorderBrush" Value="{StaticResource BorderBrush}" />
        <Setter Property="BorderThickness" Value="0 0 0 1" />
        <Setter Property="HorizontalContentAlignment" Value="Left" />
    </Style>

    <!--  DataGrid Cell Style  -->
    <Style x:Key="DataGridCellStyle" TargetType="DataGridCell">
        <Setter Property="BorderThickness" Value="0" />
        <Setter Property="Padding" Value="12 8" />
        <Setter Property="FontSize" Value="13" />
        <Setter Property="Foreground" Value="{StaticResource TextPrimaryBrush}" />
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="DataGridCell">
                    <Border Padding="{TemplateBinding Padding}"
                            Background="{TemplateBinding Background}"
                            BorderBrush="{TemplateBinding BorderBrush}"
                            BorderThickness="{TemplateBinding BorderThickness}">
                        <ContentPresenter VerticalAlignment="Center" />
                    </Border>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
        <Style.Triggers>
            <Trigger Property="IsSelected" Value="True">
                <Setter Property="Background" Value="{StaticResource PrimaryLightBrush}" />
                <Setter Property="Foreground" Value="{StaticResource PrimaryBrush}" />
                <Setter Property="FontWeight" Value="Bold" />
            </Trigger>
        </Style.Triggers>
    </Style>

    <!--  DataGrid Row Style  -->
    <Style x:Key="DataGridRowStyle" TargetType="DataGridRow">
        <Setter Property="Background" Value="{StaticResource WhiteBrush}" />
        <Style.Triggers>
            <Trigger Property="IsMouseOver" Value="True">
                <Setter Property="Background" Value="{StaticResource NeutralHoverBrush}" />
            </Trigger>
            <Trigger Property="IsSelected" Value="True">
                <Setter Property="Background" Value="{StaticResource PrimaryLightBrush}" />
            </Trigger>
        </Style.Triggers>
    </Style>

</ResourceDictionary>
```

**Step 2: Build to verify the new file compiles**

```bash
dotnet build TelAvivMuni-Exercise.sln
```

Expected: `Build succeeded.` (no errors — the file is not yet referenced by anything)

**Step 3: Commit**

```bash
git add TelAvivMuni-Exercise.Themes/Themes/Dark.Styles.xaml
git commit -m "style: add Dark.Styles.xaml with shared dark-theme named styles"
```

---

## Task 2: Refactor `GruvboxDark.Styles.xaml`

**Files:**
- Modify: `TelAvivMuni-Exercise.Themes.Zed.GruvboxDark/Themes/GruvboxDark.Styles.xaml`

**Step 1: Replace the entire file content**

The new file:
1. Merges `Dark.Styles.xaml` (adds the pack URI to `MergedDictionaries`)
2. Keeps implicit `Window`, `TextBlock`, `TextBox`, `ScrollBar`-family styles unchanged
3. Renames `GruvboxDarkBrowseButtonStyle` → `BrowseButtonStyle`
4. Renames `GruvboxDarkDataBrowserTextBoxStyle` → `DataBrowserTextBoxStyle`
5. Removes the 7 now-redundant named styles (`BaseButtonStyle`, `BlueButtonStyle`, `GrayButtonStyle`, `ClearButtonStyle`, `DataGridColumnHeaderStyle`, `DataGridCellStyle`, `DataGridRowStyle`)
6. Updates the `DataBrowserBox` implicit style: uses `ClearButtonStyle` on `PART_ClearButton` instead of an inline anonymous style, and uses `HasSelection` `ControlTemplate.Trigger` instead of a `DataTrigger` on `SelectedItem`

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:controls="clr-namespace:TelAvivMuni_Exercise.Controls;assembly=TelAvivMuni-Exercise.Controls"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <ResourceDictionary.MergedDictionaries>
        <ResourceDictionary Source="pack://application:,,,/TelAvivMuni-Exercise.Themes.Zed.GruvboxDark;component/Themes/GruvboxDark.Colors.xaml" />
        <ResourceDictionary Source="pack://application:,,,/TelAvivMuni-Exercise.Themes;component/Themes/Dark.Styles.xaml" />
    </ResourceDictionary.MergedDictionaries>

    <!--  ============================================  -->
    <!--  GruvboxDark Theme Styles — Gruvbox Dark  -->
    <!--  ============================================  -->

    <!--  ============================================  -->
    <!--  Implicit base styles (dark theme overrides)  -->
    <!--  ============================================  -->

    <!--  Window: dark background + light text  -->
    <Style TargetType="Window">
        <Setter Property="Background" Value="{StaticResource NeutralDarkBrush}" />
        <Setter Property="Foreground" Value="{StaticResource TextPrimaryBrush}" />
    </Style>

    <!--  TextBlock: inherit Gruvbox fg1  -->
    <Style TargetType="TextBlock">
        <Setter Property="Foreground" Value="{StaticResource TextPrimaryBrush}" />
    </Style>

    <!--  TextBox: dark background + light text  -->
    <Style TargetType="TextBox">
        <Setter Property="Background" Value="{StaticResource WhiteBrush}" />
        <Setter Property="Foreground" Value="{StaticResource TextPrimaryBrush}" />
        <Setter Property="CaretBrush" Value="{StaticResource TextPrimaryBrush}" />
        <Setter Property="BorderBrush" Value="{StaticResource BorderBrush}" />
        <Setter Property="SelectionBrush" Value="{StaticResource PrimaryBrush}" />
    </Style>

    <!--  ScrollBar thumb/track dark theme  -->
    <Style x:Key="ScrollBarThumbStyle" TargetType="Thumb">
        <Setter Property="OverridesDefaultStyle" Value="True" />
        <Setter Property="IsTabStop" Value="False" />
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="Thumb">
                    <Border Background="{StaticResource NeutralLightBrush}" CornerRadius="3" />
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

    <Style x:Key="ScrollBarLineButtonStyle" TargetType="RepeatButton">
        <Setter Property="OverridesDefaultStyle" Value="True" />
        <Setter Property="Focusable" Value="False" />
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="RepeatButton">
                    <Border Width="0" Height="0" />
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

    <Style x:Key="ScrollBarPageButtonStyle" TargetType="RepeatButton">
        <Setter Property="OverridesDefaultStyle" Value="True" />
        <Setter Property="IsTabStop" Value="False" />
        <Setter Property="Focusable" Value="False" />
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="RepeatButton">
                    <Border Background="Transparent" />
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

    <!--  Vertical ScrollBar  -->
    <ControlTemplate x:Key="VerticalScrollBarTemplate" TargetType="ScrollBar">
        <Grid>
            <Border Background="{StaticResource NeutralExtraLightBrush}" CornerRadius="3" />
            <Track x:Name="PART_Track" IsDirectionReversed="True">
                <Track.DecreaseRepeatButton>
                    <RepeatButton Command="ScrollBar.PageUpCommand" Style="{StaticResource ScrollBarPageButtonStyle}" />
                </Track.DecreaseRepeatButton>
                <Track.Thumb>
                    <Thumb Style="{StaticResource ScrollBarThumbStyle}" />
                </Track.Thumb>
                <Track.IncreaseRepeatButton>
                    <RepeatButton Command="ScrollBar.PageDownCommand" Style="{StaticResource ScrollBarPageButtonStyle}" />
                </Track.IncreaseRepeatButton>
            </Track>
        </Grid>
    </ControlTemplate>

    <!--  Horizontal ScrollBar  -->
    <ControlTemplate x:Key="HorizontalScrollBarTemplate" TargetType="ScrollBar">
        <Grid>
            <Border Background="{StaticResource NeutralExtraLightBrush}" CornerRadius="3" />
            <Track x:Name="PART_Track" IsDirectionReversed="False">
                <Track.DecreaseRepeatButton>
                    <RepeatButton Command="ScrollBar.PageLeftCommand" Style="{StaticResource ScrollBarPageButtonStyle}" />
                </Track.DecreaseRepeatButton>
                <Track.Thumb>
                    <Thumb Style="{StaticResource ScrollBarThumbStyle}" />
                </Track.Thumb>
                <Track.IncreaseRepeatButton>
                    <RepeatButton Command="ScrollBar.PageRightCommand" Style="{StaticResource ScrollBarPageButtonStyle}" />
                </Track.IncreaseRepeatButton>
            </Track>
        </Grid>
    </ControlTemplate>

    <!--  Implicit ScrollBar style  -->
    <Style TargetType="ScrollBar">
        <Setter Property="OverridesDefaultStyle" Value="True" />
        <Setter Property="SnapsToDevicePixels" Value="True" />
        <Style.Triggers>
            <Trigger Property="Orientation" Value="Vertical">
                <Setter Property="Width" Value="8" />
                <Setter Property="Height" Value="Auto" />
                <Setter Property="Template" Value="{StaticResource VerticalScrollBarTemplate}" />
            </Trigger>
            <Trigger Property="Orientation" Value="Horizontal">
                <Setter Property="Width" Value="Auto" />
                <Setter Property="Height" Value="8" />
                <Setter Property="Template" Value="{StaticResource HorizontalScrollBarTemplate}" />
            </Trigger>
        </Style.Triggers>
    </Style>

    <!--  ============================================  -->
    <!--  DataBrowserBox dark theme override           -->
    <!--  Overrides Generic.xaml so internal elements  -->
    <!--  use Gruvbox dark brushes.                    -->
    <!--  ============================================  -->

    <!--  Browse Button Style  -->
    <Style x:Key="BrowseButtonStyle" TargetType="Button">
        <Setter Property="Width" Value="30" />
        <Setter Property="Background" Value="{StaticResource NeutralLightBrush}" />
        <Setter Property="Foreground" Value="{StaticResource TextPrimaryBrush}" />
        <Setter Property="BorderThickness" Value="0" />
        <Setter Property="Cursor" Value="Hand" />
        <Setter Property="FontWeight" Value="Medium" />
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="Button">
                    <Border Padding="3"
                            Background="{TemplateBinding Background}"
                            BorderBrush="{StaticResource BorderBrush}"
                            BorderThickness="1 0 0 0"
                            CornerRadius="0,0,0,0">
                        <Viewbox Width="18" Height="18">
                            <Path Width="24"
                                  Height="24"
                                  Data="M 4,2 H 20 A 2,2 0 0 1 22,4 V 8 A 2,2 0 0 1 20,10 H 4 A 2,2 0 0 1 2,8 V 4 A 2,2 0 0 1 4,2 Z M 4,14 H 20 A 2,2 0 0 1 22,16 V 20 A 2,2 0 0 1 20,22 H 4 A 2,2 0 0 1 2,20 V 16 A 2,2 0 0 1 4,14 Z M 4.5,6 A 1.5,1.5 0 1,0 7.5,6 A 1.5,1.5 0 1,0 4.5,6 M 4.5,18 A 1.5,1.5 0 1,0 7.5,18 A 1.5,1.5 0 1,0 4.5,18"
                                  Fill="Transparent"
                                  Stretch="Uniform"
                                  Stroke="{StaticResource PrimaryDarkBrush}"
                                  StrokeThickness="1" />
                        </Viewbox>
                    </Border>
                    <ControlTemplate.Triggers>
                        <Trigger Property="IsMouseOver" Value="True">
                            <Setter Property="Background" Value="{StaticResource NeutralMediumBrush}" />
                        </Trigger>
                        <Trigger Property="IsPressed" Value="True">
                            <Setter Property="Background" Value="{StaticResource NeutralDarkBrush}" />
                        </Trigger>
                    </ControlTemplate.Triggers>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

    <!--  DataBrowserBox TextBox Style  -->
    <Style x:Key="DataBrowserTextBoxStyle" TargetType="TextBox">
        <Setter Property="VerticalContentAlignment" Value="Center" />
        <Setter Property="Background" Value="Transparent" />
        <Setter Property="Foreground" Value="{StaticResource TextPrimaryBrush}" />
        <Setter Property="CaretBrush" Value="{StaticResource TextPrimaryBrush}" />
        <Setter Property="BorderThickness" Value="0" />
        <Setter Property="IsReadOnly" Value="True" />
        <Setter Property="Padding" Value="5 0 25 0" />
        <Style.Triggers>
            <Trigger Property="Opacity" Value="0.5">
                <Setter Property="FontWeight" Value="Normal" />
                <Setter Property="FontStyle" Value="Italic" />
            </Trigger>
            <Trigger Property="Opacity" Value="1">
                <Setter Property="FontWeight" Value="Bold" />
                <Setter Property="FontStyle" Value="Normal" />
            </Trigger>
        </Style.Triggers>
    </Style>

    <!--  DataBrowserBox implicit style override (dark)  -->
    <Style TargetType="{x:Type controls:DataBrowserBox}">
        <Setter Property="Background" Value="{StaticResource WhiteBrush}" />
        <Setter Property="Foreground" Value="{StaticResource TextPrimaryBrush}" />
        <Setter Property="BorderBrush" Value="{StaticResource BorderLightBrush}" />
        <Setter Property="BorderThickness" Value="1" />
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="{x:Type controls:DataBrowserBox}">
                    <Border Background="{TemplateBinding Background}"
                            BorderBrush="{TemplateBinding BorderBrush}"
                            BorderThickness="{TemplateBinding BorderThickness}">
                        <Grid>
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="Auto" />
                                <ColumnDefinition Width="*" />
                                <ColumnDefinition Width="Auto" />
                            </Grid.ColumnDefinitions>
                            <!--  Selected product general icon  -->
                            <Viewbox Grid.Column="0"
                                     Width="18"
                                     Height="18"
                                     Margin="5 0 0 0"
                                     VerticalAlignment="Center">
                                <Path Data="M21 8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16Z M3.29 7 L12 12 L20.71 7 M12 22 L12 12"
                                      Fill="Transparent"
                                      Stretch="Uniform"
                                      Stroke="{StaticResource PrimaryDarkBrush}"
                                      StrokeEndLineCap="Round"
                                      StrokeLineJoin="Round"
                                      StrokeStartLineCap="Round"
                                      StrokeThickness="2" />
                            </Viewbox>
                            <!--  TextBox + clear button overlay  -->
                            <Grid Grid.Column="1">
                                <TextBox x:Name="PART_TextBox"
                                         Opacity="0.5"
                                         Style="{StaticResource DataBrowserTextBoxStyle}"
                                         Text="Click to select..." />
                                <Button x:Name="PART_ClearButton"
                                        Margin="5 0 5 0"
                                        HorizontalAlignment="Right"
                                        VerticalAlignment="Center"
                                        Style="{StaticResource ClearButtonStyle}" />
                            </Grid>
                            <Button x:Name="PART_BrowseButton"
                                    Grid.Column="2"
                                    Style="{StaticResource BrowseButtonStyle}" />
                        </Grid>
                    </Border>
                    <ControlTemplate.Triggers>
                        <!--
                            A ControlTemplate Trigger tracks the DP via the property engine,
                            guaranteeing change-notification reliability — unlike a DataTrigger
                            binding on SelectedItem inside a Button.Style.
                        -->
                        <Trigger Property="HasSelection" Value="False">
                            <Setter TargetName="PART_ClearButton" Property="Visibility" Value="Collapsed" />
                        </Trigger>
                    </ControlTemplate.Triggers>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

</ResourceDictionary>
```

**Step 2: Build to verify**

```bash
dotnet build TelAvivMuni-Exercise.sln
```

Expected: `Build succeeded.`

**Step 3: Commit**

```bash
git add TelAvivMuni-Exercise.Themes.Zed.GruvboxDark/Themes/GruvboxDark.Styles.xaml
git commit -m "style: refactor GruvboxDark — use shared Dark.Styles, rename BrowseButtonStyle/DataBrowserTextBoxStyle, fix DataBrowserBox HasSelection trigger"
```

---

## Task 3: Refactor `AyuDark.Styles.xaml`

**Files:**
- Modify: `TelAvivMuni-Exercise.Themes.Zed.AyuDark/Themes/AyuDark.Styles.xaml`

**Step 1: Replace the entire file content**

Apply the identical changes as Task 2, replacing `GruvboxDark` pack URI with the AyuDark one, and updating the comment header. The result:

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:controls="clr-namespace:TelAvivMuni_Exercise.Controls;assembly=TelAvivMuni-Exercise.Controls"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <ResourceDictionary.MergedDictionaries>
        <ResourceDictionary Source="pack://application:,,,/TelAvivMuni-Exercise.Themes.Zed.AyuDark;component/Themes/AyuDark.Colors.xaml" />
        <ResourceDictionary Source="pack://application:,,,/TelAvivMuni-Exercise.Themes;component/Themes/Dark.Styles.xaml" />
    </ResourceDictionary.MergedDictionaries>

    <!--  ============================================  -->
    <!--  AyuDark Theme Styles — Ayu Dark             -->
    <!--  ============================================  -->

    <!--  ============================================  -->
    <!--  Implicit base styles (dark theme overrides)  -->
    <!--  ============================================  -->

    <!--  Window: dark background + light text  -->
    <Style TargetType="Window">
        <Setter Property="Background" Value="{StaticResource NeutralDarkBrush}" />
        <Setter Property="Foreground" Value="{StaticResource TextPrimaryBrush}" />
    </Style>

    <!--  TextBlock: inherit Ayu text  -->
    <Style TargetType="TextBlock">
        <Setter Property="Foreground" Value="{StaticResource TextPrimaryBrush}" />
    </Style>

    <!--  TextBox: dark background + light text  -->
    <Style TargetType="TextBox">
        <Setter Property="Background" Value="{StaticResource WhiteBrush}" />
        <Setter Property="Foreground" Value="{StaticResource TextPrimaryBrush}" />
        <Setter Property="CaretBrush" Value="{StaticResource TextPrimaryBrush}" />
        <Setter Property="BorderBrush" Value="{StaticResource BorderBrush}" />
        <Setter Property="SelectionBrush" Value="{StaticResource PrimaryBrush}" />
    </Style>

    <!--  ScrollBar thumb/track dark theme  -->
    <Style x:Key="ScrollBarThumbStyle" TargetType="Thumb">
        <Setter Property="OverridesDefaultStyle" Value="True" />
        <Setter Property="IsTabStop" Value="False" />
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="Thumb">
                    <Border Background="{StaticResource NeutralLightBrush}" CornerRadius="3" />
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

    <Style x:Key="ScrollBarLineButtonStyle" TargetType="RepeatButton">
        <Setter Property="OverridesDefaultStyle" Value="True" />
        <Setter Property="Focusable" Value="False" />
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="RepeatButton">
                    <Border Width="0" Height="0" />
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

    <Style x:Key="ScrollBarPageButtonStyle" TargetType="RepeatButton">
        <Setter Property="OverridesDefaultStyle" Value="True" />
        <Setter Property="IsTabStop" Value="False" />
        <Setter Property="Focusable" Value="False" />
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="RepeatButton">
                    <Border Background="Transparent" />
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

    <!--  Vertical ScrollBar  -->
    <ControlTemplate x:Key="VerticalScrollBarTemplate" TargetType="ScrollBar">
        <Grid>
            <Border Background="{StaticResource NeutralExtraLightBrush}" CornerRadius="3" />
            <Track x:Name="PART_Track" IsDirectionReversed="True">
                <Track.DecreaseRepeatButton>
                    <RepeatButton Command="ScrollBar.PageUpCommand" Style="{StaticResource ScrollBarPageButtonStyle}" />
                </Track.DecreaseRepeatButton>
                <Track.Thumb>
                    <Thumb Style="{StaticResource ScrollBarThumbStyle}" />
                </Track.Thumb>
                <Track.IncreaseRepeatButton>
                    <RepeatButton Command="ScrollBar.PageDownCommand" Style="{StaticResource ScrollBarPageButtonStyle}" />
                </Track.IncreaseRepeatButton>
            </Track>
        </Grid>
    </ControlTemplate>

    <!--  Horizontal ScrollBar  -->
    <ControlTemplate x:Key="HorizontalScrollBarTemplate" TargetType="ScrollBar">
        <Grid>
            <Border Background="{StaticResource NeutralExtraLightBrush}" CornerRadius="3" />
            <Track x:Name="PART_Track" IsDirectionReversed="False">
                <Track.DecreaseRepeatButton>
                    <RepeatButton Command="ScrollBar.PageLeftCommand" Style="{StaticResource ScrollBarPageButtonStyle}" />
                </Track.DecreaseRepeatButton>
                <Track.Thumb>
                    <Thumb Style="{StaticResource ScrollBarThumbStyle}" />
                </Track.Thumb>
                <Track.IncreaseRepeatButton>
                    <RepeatButton Command="ScrollBar.PageRightCommand" Style="{StaticResource ScrollBarPageButtonStyle}" />
                </Track.IncreaseRepeatButton>
            </Track>
        </Grid>
    </ControlTemplate>

    <!--  Implicit ScrollBar style  -->
    <Style TargetType="ScrollBar">
        <Setter Property="OverridesDefaultStyle" Value="True" />
        <Setter Property="SnapsToDevicePixels" Value="True" />
        <Style.Triggers>
            <Trigger Property="Orientation" Value="Vertical">
                <Setter Property="Width" Value="8" />
                <Setter Property="Height" Value="Auto" />
                <Setter Property="Template" Value="{StaticResource VerticalScrollBarTemplate}" />
            </Trigger>
            <Trigger Property="Orientation" Value="Horizontal">
                <Setter Property="Width" Value="Auto" />
                <Setter Property="Height" Value="8" />
                <Setter Property="Template" Value="{StaticResource HorizontalScrollBarTemplate}" />
            </Trigger>
        </Style.Triggers>
    </Style>

    <!--  ============================================  -->
    <!--  DataBrowserBox dark theme override           -->
    <!--  Overrides Generic.xaml so internal elements  -->
    <!--  use Ayu Dark brushes.                        -->
    <!--  ============================================  -->

    <!--  Browse Button Style  -->
    <Style x:Key="BrowseButtonStyle" TargetType="Button">
        <Setter Property="Width" Value="30" />
        <Setter Property="Background" Value="{StaticResource NeutralLightBrush}" />
        <Setter Property="Foreground" Value="{StaticResource TextPrimaryBrush}" />
        <Setter Property="BorderThickness" Value="0" />
        <Setter Property="Cursor" Value="Hand" />
        <Setter Property="FontWeight" Value="Medium" />
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="Button">
                    <Border Padding="3"
                            Background="{TemplateBinding Background}"
                            BorderBrush="{StaticResource BorderBrush}"
                            BorderThickness="1 0 0 0"
                            CornerRadius="0,0,0,0">
                        <Viewbox Width="18" Height="18">
                            <Path Width="24"
                                  Height="24"
                                  Data="M 4,2 H 20 A 2,2 0 0 1 22,4 V 8 A 2,2 0 0 1 20,10 H 4 A 2,2 0 0 1 2,8 V 4 A 2,2 0 0 1 4,2 Z M 4,14 H 20 A 2,2 0 0 1 22,16 V 20 A 2,2 0 0 1 20,22 H 4 A 2,2 0 0 1 2,20 V 16 A 2,2 0 0 1 4,14 Z M 4.5,6 A 1.5,1.5 0 1,0 7.5,6 A 1.5,1.5 0 1,0 4.5,6 M 4.5,18 A 1.5,1.5 0 1,0 7.5,18 A 1.5,1.5 0 1,0 4.5,18"
                                  Fill="Transparent"
                                  Stretch="Uniform"
                                  Stroke="{StaticResource PrimaryDarkBrush}"
                                  StrokeThickness="1" />
                        </Viewbox>
                    </Border>
                    <ControlTemplate.Triggers>
                        <Trigger Property="IsMouseOver" Value="True">
                            <Setter Property="Background" Value="{StaticResource NeutralMediumBrush}" />
                        </Trigger>
                        <Trigger Property="IsPressed" Value="True">
                            <Setter Property="Background" Value="{StaticResource NeutralDarkBrush}" />
                        </Trigger>
                    </ControlTemplate.Triggers>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

    <!--  DataBrowserBox TextBox Style  -->
    <Style x:Key="DataBrowserTextBoxStyle" TargetType="TextBox">
        <Setter Property="VerticalContentAlignment" Value="Center" />
        <Setter Property="Background" Value="Transparent" />
        <Setter Property="Foreground" Value="{StaticResource TextPrimaryBrush}" />
        <Setter Property="CaretBrush" Value="{StaticResource TextPrimaryBrush}" />
        <Setter Property="BorderThickness" Value="0" />
        <Setter Property="IsReadOnly" Value="True" />
        <Setter Property="Padding" Value="5 0 25 0" />
        <Style.Triggers>
            <Trigger Property="Opacity" Value="0.5">
                <Setter Property="FontWeight" Value="Normal" />
                <Setter Property="FontStyle" Value="Italic" />
            </Trigger>
            <Trigger Property="Opacity" Value="1">
                <Setter Property="FontWeight" Value="Bold" />
                <Setter Property="FontStyle" Value="Normal" />
            </Trigger>
        </Style.Triggers>
    </Style>

    <!--  DataBrowserBox implicit style override (dark)  -->
    <Style TargetType="{x:Type controls:DataBrowserBox}">
        <Setter Property="Background" Value="{StaticResource WhiteBrush}" />
        <Setter Property="Foreground" Value="{StaticResource TextPrimaryBrush}" />
        <Setter Property="BorderBrush" Value="{StaticResource BorderLightBrush}" />
        <Setter Property="BorderThickness" Value="1" />
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="{x:Type controls:DataBrowserBox}">
                    <Border Background="{TemplateBinding Background}"
                            BorderBrush="{TemplateBinding BorderBrush}"
                            BorderThickness="{TemplateBinding BorderThickness}">
                        <Grid>
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="Auto" />
                                <ColumnDefinition Width="*" />
                                <ColumnDefinition Width="Auto" />
                            </Grid.ColumnDefinitions>
                            <!--  Selected product general icon  -->
                            <Viewbox Grid.Column="0"
                                     Width="18"
                                     Height="18"
                                     Margin="5 0 0 0"
                                     VerticalAlignment="Center">
                                <Path Data="M21 8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16Z M3.29 7 L12 12 L20.71 7 M12 22 L12 12"
                                      Fill="Transparent"
                                      Stretch="Uniform"
                                      Stroke="{StaticResource PrimaryDarkBrush}"
                                      StrokeEndLineCap="Round"
                                      StrokeLineJoin="Round"
                                      StrokeStartLineCap="Round"
                                      StrokeThickness="2" />
                            </Viewbox>
                            <!--  TextBox + clear button overlay  -->
                            <Grid Grid.Column="1">
                                <TextBox x:Name="PART_TextBox"
                                         Opacity="0.5"
                                         Style="{StaticResource DataBrowserTextBoxStyle}"
                                         Text="Click to select..." />
                                <Button x:Name="PART_ClearButton"
                                        Margin="5 0 5 0"
                                        HorizontalAlignment="Right"
                                        VerticalAlignment="Center"
                                        Style="{StaticResource ClearButtonStyle}" />
                            </Grid>
                            <Button x:Name="PART_BrowseButton"
                                    Grid.Column="2"
                                    Style="{StaticResource BrowseButtonStyle}" />
                        </Grid>
                    </Border>
                    <ControlTemplate.Triggers>
                        <!--
                            A ControlTemplate Trigger tracks the DP via the property engine,
                            guaranteeing change-notification reliability — unlike a DataTrigger
                            binding on SelectedItem inside a Button.Style.
                        -->
                        <Trigger Property="HasSelection" Value="False">
                            <Setter TargetName="PART_ClearButton" Property="Visibility" Value="Collapsed" />
                        </Trigger>
                    </ControlTemplate.Triggers>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

</ResourceDictionary>
```

**Step 2: Build to verify**

```bash
dotnet build TelAvivMuni-Exercise.sln
```

Expected: `Build succeeded.`

**Step 3: Commit**

```bash
git add TelAvivMuni-Exercise.Themes.Zed.AyuDark/Themes/AyuDark.Styles.xaml
git commit -m "style: refactor AyuDark — use shared Dark.Styles, rename BrowseButtonStyle/DataBrowserTextBoxStyle, fix DataBrowserBox HasSelection trigger"
```

---

## Task 4: Rename `BlueButtonStyle` → `PrimaryButtonStyle` in `Blue.Styles.xaml`

**Files:**
- Modify: `TelAvivMuni-Exercise.Themes.Blue/Themes/Blue.Styles.xaml`

**Step 1: Rename the style key**

Find:
```xml
<!--  Blue Button Style (Primary actions)  -->
<Style x:Key="BlueButtonStyle"
```

Replace with:
```xml
<!--  Primary Button Style (Primary actions)  -->
<Style x:Key="PrimaryButtonStyle"
```

(The `BasedOn`, `TargetType`, and all setters remain unchanged.)

**Step 2: Build to verify**

```bash
dotnet build TelAvivMuni-Exercise.sln
```

Expected: `Build succeeded.`

**Step 3: Commit**

```bash
git add TelAvivMuni-Exercise.Themes.Blue/Themes/Blue.Styles.xaml
git commit -m "style: rename BlueButtonStyle -> PrimaryButtonStyle in Blue theme"
```

---

## Task 5: Rename `BlueButtonStyle` → `PrimaryButtonStyle` in `Emerald.Styles.xaml`

**Files:**
- Modify: `TelAvivMuni-Exercise.Themes.Emerald/Themes/Emerald.Styles.xaml`

**Step 1: Rename the style key**

Find:
```xml
<!--  Blue Button Style (Primary actions) - Using Emerald  -->
<Style x:Key="BlueButtonStyle"
```

Replace with:
```xml
<!--  Primary Button Style (Primary actions)  -->
<Style x:Key="PrimaryButtonStyle"
```

**Step 2: Build to verify**

```bash
dotnet build TelAvivMuni-Exercise.sln
```

Expected: `Build succeeded.`

**Step 3: Commit**

```bash
git add TelAvivMuni-Exercise.Themes.Emerald/Themes/Emerald.Styles.xaml
git commit -m "style: rename BlueButtonStyle -> PrimaryButtonStyle in Emerald theme"
```

---

## Task 6: Update `DataBrowserDialog.xaml` — the one `BlueButtonStyle` consumer

**Files:**
- Modify: `TelAvivMuni-Exercise.Controls/DataBrowserDialog.xaml:182`

**Step 1: Update the style reference**

Find (line 182):
```xml
Style="{StaticResource BlueButtonStyle}" />
```

Replace with:
```xml
Style="{StaticResource PrimaryButtonStyle}" />
```

**Step 2: Build to verify — this is the final build; all changes are now in place**

```bash
dotnet build TelAvivMuni-Exercise.sln
```

Expected: `Build succeeded.` with 0 errors.

**Step 3: Run tests to confirm no regressions**

```bash
dotnet test TelAvivMuni-Exercise.Tests/TelAvivMuni-Exercise.Tests.csproj
```

Expected: All tests pass (unit tests cover ViewModels, not XAML rendering).

**Step 4: Commit**

```bash
git add TelAvivMuni-Exercise.Controls/DataBrowserDialog.xaml
git commit -m "style: update DataBrowserDialog to use PrimaryButtonStyle"
```

---

## Verification checklist

- [ ] `dotnet build TelAvivMuni-Exercise.sln` → 0 errors after each task
- [ ] All unit tests pass after Task 6
- [ ] No `BlueButtonStyle` references remain: `grep -r "BlueButtonStyle" --include="*.xaml"` → no output
- [ ] No theme-prefixed browse/textbox style keys remain: `grep -r "GruvboxDarkBrowseButton\|AyuDarkBrowseButton\|GruvboxDarkDataBrowser\|AyuDarkDataBrowser" --include="*.xaml"` → no output
- [ ] `Dark.Styles.xaml` is referenced by exactly both dark themes
- [ ] Visual smoke test: launch the app in each theme (Blue, Emerald, Gruvbox, Ayu); confirm `DataBrowserBox` clear button shows/hides correctly when selecting/clearing a product
