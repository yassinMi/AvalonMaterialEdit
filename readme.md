## Usage

App.xaml:

```xml
<ResourceDictionary.MergedDictionaries>

    <ResourceDictionary Source="pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml" />

</ResourceDictionary.MergedDictionaries>

```

MainWindow.xaml:
```xml
<Window
        xmlns:ame="clr-namespace:AvalonMaterialEdit;assembly=AvalonMaterialEdit"
        >
```

**example**

```xml
<avalonEdit:TextEditor materialDesign:HintAssist.Hint="Enter Custom Filter" ame:TextEditorAssist.FieldDescriptors="{x:Static local:MainWindow.FieldDescriptors}" ame:TextEditorAssist.HighlightingDefinitionGroupName="products" ame:TextEditorAssist.Mode="SqlFilterExpession" materialDesign:HintAssist.HelperText="Example: Email NOT NULL AND TotalAmount > 20"  />
```

![DemoApp1](https://github.com/yassinMi/AvalonMaterialEdit/blob/main/Demo/img/DemoApp1.png?raw=true)
