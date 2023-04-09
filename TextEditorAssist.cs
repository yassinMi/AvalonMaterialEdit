using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace AvalonMaterialEdit
{
    public static class TextEditorAssist
    {






        public static bool GetAcceptReturn(DependencyObject obj)
        {
            return (bool)obj.GetValue(AcceptReturnProperty);
        }

        public static void SetAcceptReturn(DependencyObject obj, bool value)
        {
            obj.SetValue(AcceptReturnProperty, value);
        }

        // Using a DependencyProperty as the backing store for AcceptReturn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AcceptReturnProperty =
            DependencyProperty.RegisterAttached("AcceptReturn", typeof(bool), typeof(TextEditorAssist), new PropertyMetadata(false));



        private static TextEditor GetTextEditor(DependencyObject obj)
        {
            return (TextEditor)obj.GetValue(TextEditorProperty);
        }

        private static void SetTextEditor(DependencyObject obj, TextEditor value)
        {
            obj.SetValue(TextEditorProperty, value);
        }

        // Using a DependencyProperty as the backing store for TextArea.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty TextEditorProperty =
            DependencyProperty.RegisterAttached("TextEditor", typeof(TextEditor), typeof(TextEditorAssist), new PropertyMetadata(null));



        /// <summary>
        /// genrates and caches or uses cache to assign a FieldsSet-Specific HighligtingDefinition to a TextEditor based on it's FieldDescriptors and HighlightingDefinitionGroupName properties
        /// </summary>
        /// <param name="targetTextEditor"></param>
        private static void TryInitializeHighlitingDefintion(TextEditor targetTextEditor)
        {
            var maybeGroupName = GetHighlightingDefinitionGroupName(targetTextEditor);
            if (maybeGroupName != null)
            {
                IHighlightingDefinition _highlight;
                if (HighlightingDefinitions.TryGetValue(maybeGroupName,out _highlight))
                {
                    targetTextEditor.SyntaxHighlighting = _highlight;
                    return;
                }
                else
                {
                    //load and store
                    var fieldDescriptors = GetFieldDescriptors(targetTextEditor);
                    if (fieldDescriptors == null) return;
                    IHighlightingDefinition TESpecificSqliteHighlightingDefinition;
                    using (var stream = System.Reflection.Assembly.GetCallingAssembly().GetManifestResourceStream("AvalonMaterialEdit.AvalonEditResources.Sqlite.xshd"))
                    {
                        using (var reader = new System.Xml.XmlTextReader(stream))
                        {
                            var SqliteHighlightingDefinition_Xshd = HighlightingLoader.LoadXshd(reader);
                            AddApplicationFieldsAsKeywords(SqliteHighlightingDefinition_Xshd, fieldDescriptors);

                           TESpecificSqliteHighlightingDefinition =
                           HighlightingLoader.Load(SqliteHighlightingDefinition_Xshd, HighlightingManager.Instance);
                        }
                    }
                    HighlightingDefinitions[maybeGroupName] = TESpecificSqliteHighlightingDefinition;
                    targetTextEditor.SyntaxHighlighting = TESpecificSqliteHighlightingDefinition;
                }
            }
        }
        /// <summary>
        /// helper for TryInitializeHighlitingDefintion()
        /// </summary>
        /// <param name="targetXshd"></param>
        /// <param name="fs"></param>
        public static void AddApplicationFieldsAsKeywords(XshdSyntaxDefinition targetXshd, IEnumerable fs)
        {
            XshdKeywords appFsKeyWords = new XshdKeywords();
            foreach (var item in fs)
            {
                appFsKeyWords.Words.Add(GetFieldDescriptorsName?.Invoke(item)??item.ToString());
            }
            appFsKeyWords.ColorReference = new XshdReference<XshdColor>(null, "Keyword1");

            XshdRuleSet mainRuleSet = targetXshd.Elements.OfType<XshdRuleSet>().Where(o => string.IsNullOrEmpty(o.Name)).First();
            mainRuleSet.Elements.Add(appFsKeyWords);
        }

        static Dictionary<object, IHighlightingDefinition> HighlightingDefinitions { get; set; } = new Dictionary<object, IHighlightingDefinition>();



        public static Func<object,string> GetFieldDescriptorsName { get; set; }



        public static string GetHighlightingDefinitionGroupName(DependencyObject obj)
        {
            return (string)obj.GetValue(HighlightingDefinitionGroupNameProperty);
        }

        public static void SetHighlightingDefinitionGroupName(DependencyObject obj, string value)
        {
            obj.SetValue(HighlightingDefinitionGroupNameProperty, value);
        }

        // Using a DependencyProperty as the backing store for HighlightingDefinitionGroupName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightingDefinitionGroupNameProperty =
            DependencyProperty.RegisterAttached("HighlightingDefinitionGroupName", typeof(string), typeof(TextEditorAssist), new PropertyMetadata(null));




        public static IEnumerable GetFieldDescriptors(DependencyObject obj)
        {
            return (IEnumerable)obj.GetValue(FieldDescriptorsProperty);
        }

        public static void SetFieldDescriptors(DependencyObject obj, IEnumerable value)
        {
            obj.SetValue(FieldDescriptorsProperty, value);
        }

        // Using a DependencyProperty as the backing store for FieldDescriptors.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FieldDescriptorsProperty =
            DependencyProperty.RegisterAttached("FieldDescriptors", typeof(IEnumerable), typeof(TextEditorAssist), new PropertyMetadata(null,h_fsChage));

        private static void h_fsChage(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var te = d as TextEditor;
            if (te == null) return;
            if (e.NewValue != null)
            {
                TryInitializeHighlitingDefintion(te);
            }
        }

        public static TextEditorMode GetMode(DependencyObject obj)
        {
            return (TextEditorMode)obj.GetValue(ModeProperty);
        }

        public static void SetMode(DependencyObject obj, TextEditorMode value)
        {
            obj.SetValue(ModeProperty, value);
        }

        // Using a DependencyProperty as the backing store for Mode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.RegisterAttached("Mode", typeof(TextEditorMode), typeof(TextEditorAssist), new PropertyMetadata(TextEditorMode.Custom,h_ModeChanged));

        private static void h_ModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var te = d as TextEditor;
            if (te == null) return;
            if((TextEditorMode)e.NewValue == TextEditorMode.SqlFilterExpession)
            {
                TryInitializeHighlitingDefintion(te);
            }
            
        }

        public static CompletionWindow GetCompletionWindow(DependencyObject obj)
        {
            return (CompletionWindow)obj.GetValue(CompletionWindowProperty);
        }

        public static void SetCompletionWindow(DependencyObject obj, CompletionWindow value)
        {
            obj.SetValue(CompletionWindowProperty, value);
        }

        // Using a DependencyProperty as the backing store for CompletionWindow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CompletionWindowProperty =
            DependencyProperty.RegisterAttached("CompletionWindow", typeof(CompletionWindow), typeof(TextEditorAssist), new PropertyMetadata(null));




        public static bool GetIsTextAreaFocused(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsTextAreaFocusedProperty);
        }

        public static void SetIsTextAreaFocused(DependencyObject obj, bool value)
        {
            obj.SetValue(IsTextAreaFocusedProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsTextAreaFocused.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsTextAreaFocusedProperty =
            DependencyProperty.RegisterAttached("IsTextAreaFocused", typeof(bool), typeof(TextEditorAssist), new PropertyMetadata(false));





        public static Brush GetSelectionBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(SelectionBrushProperty);
        }

        public static void SetSelectionBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(SelectionBrushProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectionBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionBrushProperty =
            DependencyProperty.RegisterAttached("SelectionBrush", typeof(Brush), typeof(TextEditorAssist), new PropertyMetadata(null,hndlSelectioBrushAttachedChanged));

        private static void hndlSelectioBrushAttachedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var te = d as TextEditor;
            if (te == null) return;
            var selectionBrush = e.NewValue as SolidColorBrush;
            if (selectionBrush == null) return;
            selectionBrush = selectionBrush.Clone();
            selectionBrush.Opacity = (double)TextBoxBase.SelectionOpacityProperty.DefaultMetadata.DefaultValue;
            te.TextArea.SelectionBrush = selectionBrush;
            te.TextArea.SelectionForeground = new SolidColorBrush(Blend((te.Foreground as SolidColorBrush).Color, selectionBrush.Color, selectionBrush.Opacity));

        }
        public static Color Blend(this Color color, Color backColor, double amount)
        {
            byte r = (byte)(color.R * amount + backColor.R * (1 - amount));
            byte g = (byte)(color.G * amount + backColor.G * (1 - amount));
            byte b = (byte)(color.B * amount + backColor.B * (1 - amount));
            return Color.FromRgb(r, g, b);
        }

        public static Brush GetCaretBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(CaretBrushProperty);
        }

        public static void SetCaretBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(CaretBrushProperty, value);
        }

        // Using a DependencyProperty as the backing store for CaretBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CaretBrushProperty =
            DependencyProperty.RegisterAttached("CaretBrush", typeof(Brush), typeof(TextEditorAssist), new PropertyMetadata(null,hndlCaretBrushAttachedChanged));

        private static void hndlCaretBrushAttachedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var te = d as TextEditor;
            if (te == null) return;
            te.TextArea.Caret.CaretBrush = e.NewValue as Brush;
        }

        public static bool GetUseTextBinding(DependencyObject obj)
        {
            return (bool)obj.GetValue(UseTextBindingProperty);
        }

        public static void SetUseTextBinding(DependencyObject obj, bool value)
        {
            obj.SetValue(UseTextBindingProperty, value);
        }

        // Using a DependencyProperty as the backing store for UseTextBinding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseTextBindingProperty =
            DependencyProperty.RegisterAttached("UseTextBinding", typeof(bool), typeof(TextEditorAssist), new PropertyMetadata(false,hndlUseTextBindingChanged));

        private static void hndlUseTextBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var te = d as TextEditor;
            if (te != null)
            {
                if((bool)e.OldValue==false && (bool)e.NewValue == true)
                {
                    //activate
                    te.TextChanged += hndlTextEditoTextChanged;
                    te.TextArea.SelectionBorder = null;
                    te.TextArea.SelectionCornerRadius = 0;
                    te.WordWrap = false;                    
                    te.PreviewKeyDown += (s, ee) => {
                        if(ee.Key== System.Windows.Input.Key.Tab)
                        {
                            ee.Handled = true;
                            te.MoveFocus( new System.Windows.Input.TraversalRequest(System.Windows.Input.FocusNavigationDirection.Next));
                            te.TextArea.MoveFocus(new System.Windows.Input.TraversalRequest(System.Windows.Input.FocusNavigationDirection.Next));
                        }
                        if (ee.Key == System.Windows.Input.Key.Enter)
                        {
                            //ee.Handled = true;
                        }
                    };
                    te.IsKeyboardFocusWithinChanged += (s, ee) =>
                    {
                        if((bool)ee.NewValue==true && (bool)ee.OldValue == false)
                        {
                            //VisualStateManager.GoToState(te, "TextAreaFocused", true);
                            
                        }
                    };

                    //CompletionWindow.StyleProperty.OverrideMetadata(typeof(CompletionWindow),new  PropertyMetadata((Style) App.Current.FindResource("MaterialDesignWindow")));

                    //# register te events:
                    SetTextEditor(te.TextArea, te);
                    te.TextArea.TextEntering += textEditor_TextArea_TextEntering;
                    te.TextArea.TextEntered += textEditor_TextArea_TextEntered;
                    te.MouseLeftButtonDown += h_MouseLeftButtonDown;
                    te.PreviewMouseLeftButtonDown += h_PrevMouseLeftButtonDown;



                }
                else if ((bool)e.OldValue == true && (bool)e.NewValue == false)
                {
                    //deactivate
                    te.TextChanged -= hndlTextEditoTextChanged;

                }
            }
        }

        private static void hndlTextEditoTextChanged(object sender, EventArgs e)
        {
            var te = sender as TextEditor;
            if (te == null) return;
            if (GetText(te) != te.Text)
            {
                SetText(te, te.Text);
            }
        }

        public static string GetText(DependencyObject obj)
        {
            return (string)obj.GetValue(TextProperty);
        }

        public static void SetText(DependencyObject obj, string value)
        {
            obj.SetValue(TextProperty, value);
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached("Text", typeof(string), typeof(TextEditorAssist), new PropertyMetadata(null,hndlTextAttachedChanged));

        private static void hndlTextAttachedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var te = d as ICSharpCode.AvalonEdit.TextEditor;
            if (te != null)
            {
                var NewValue = e.NewValue.ToString();
                if (te.Text!= NewValue)
                te.Text = NewValue;
            }
        }

        public static string GetSyntaxHighlighterSource(DependencyObject obj)
        {
            return (string)obj.GetValue(SyntaxHighlighterSourceProperty);
        }

        public static void SetSyntaxHighlighterSource(DependencyObject obj, string value)
        {
            obj.SetValue(SyntaxHighlighterSourceProperty, value);
        }

        // Using a DependencyProperty as the backing store for SyntaxHighlighterSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SyntaxHighlighterSourceProperty =
            DependencyProperty.RegisterAttached("SyntaxHighlighterSource", typeof(string), typeof(TextEditorAssist), new PropertyMetadata(null));





        static string[] sql_keywords = new string[] {
             "SELECT",
    "FROM",
    "WHERE",
    "GROUP BY",
    "ORDER BY",
    "LIMIT",
    "OFFSET",
    "INNER JOIN",
    "LEFT JOIN",
    "RIGHT JOIN",
    "FULL JOIN",
    "UNION",
    "EXCEPT",
    "INTERSECT",
    "IF",
    "CASE",
    "WHEN",
    "THEN",
    "ELSE",
    "END",
    "AND",
    "OR",
    "IN",
    "BETWEEN",
    "LIKE"};




        #region te_events_handlers

        static private void h_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var te = sender as TextEditor;
            te?.Focus();
        }
        static private void h_PrevMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           
            CompletionWindow completionWindow = GetCompletionWindow(sender as DependencyObject);
            if (completionWindow != null)
            {
                completionWindow.Close();
            }
        }




        public static T FindAncestor<T>(FrameworkElement obj)
    where T : FrameworkElement
        {
            if (obj != null)
            {
                var dependObj = obj;
                do
                {
                    dependObj = (dependObj.Parent as FrameworkElement);
                    if (dependObj is T)
                        return dependObj as T;
                }
                while (dependObj != null);
            }

            return null;
        }

        static void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            TextArea textArea = sender as TextArea;
            TextEditor userSqliteTextEditor = textArea==null? null : GetTextEditor(textArea);

            if (userSqliteTextEditor == null) return;
            var SqliteHighlightingDefinition = userSqliteTextEditor.SyntaxHighlighting;
            if (SqliteHighlightingDefinition == null) return;

            CompletionWindow completionWindow = GetCompletionWindow(sender as DependencyObject);
            Debug.WriteLine($"e- Strt: {completionWindow?.StartOffset}");
            Debug.WriteLine($"End    : {completionWindow?.EndOffset}");
            Debug.WriteLine($"txt    : {e.Text}");

            if (completionWindow == null)
            {
                //#todo: don't open if we're in a string token (a " or ' is currently open)
                //this informationg should be obtained from the highlighter
                bool isInsideStringToken = false;
                var tc = userSqliteTextEditor.Document;
                IHighlighter highlighter = new DocumentHighlighter(tc, SqliteHighlightingDefinition);
                var caret_offset = userSqliteTextEditor.CaretOffset;
                var caret_loc = tc.GetLocation(caret_offset);
                var currLine = caret_loc.Line;
                var highlightedLine = highlighter.HighlightLine(currLine);
                var pos_in_line = caret_loc.Column;
                var curr_sect = highlightedLine.Sections.Where(sect => (sect.Offset <= caret_offset && (sect.Offset + sect.Length) >= caret_offset)).LastOrDefault();

                if (curr_sect != null)
                {
                    if (curr_sect.Color.Name == "String" || curr_sect.Color.Name == "String2")
                    {
                        isInsideStringToken = true;
                    }
                }



                if (isInsideStringToken)
                {
                    return;

                }


                //#don't open the window if the last character (before the one inserted) is not a space or a line start
                var previous_char_pos = userSqliteTextEditor.CaretOffset - 2;
                try
                {
                    char previous_char = userSqliteTextEditor.Text[previous_char_pos];
                    if (char.IsLetterOrDigit(previous_char))
                    {
                        return;
                    }
                }
                catch
                {

                }


                //var maybe_fields = GetFieldDescriptors(userSqliteTextEditor).Where(f => f.PropName.ToLower().StartsWith(e.Text.ToLower()));
                var maybe_fields = GetFieldDescriptors(userSqliteTextEditor).Cast<object>(). Where(f => (GetFieldDescriptorsName?.Invoke(f)?? f.ToString()).ToLower().StartsWith(e.Text.ToLower()));
                //var maybe_fields = GetFieldDescriptors(userSqliteTextEditor).Where(f=>FieldDescriptorsCompletionsPredicate(f, e.Text));
                var maybe_sql_keywords = sql_keywords.Where(f => f.ToLower().StartsWith(e.Text.ToLower()));

                IEnumerable<object> allTokens = maybe_fields.Cast<object>().Concat(maybe_sql_keywords);
                if (allTokens.Any())
                {
                    // Open code completion after the user has pressed dot:
                    completionWindow = new CompletionWindow(userSqliteTextEditor.TextArea);
                    completionWindow.AllowsTransparency = true;
                    completionWindow.Background = Brushes.Transparent;
                    SetCompletionWindow(userSqliteTextEditor, completionWindow);
                    completionWindow.WindowStyle = WindowStyle.None;
                    completionWindow.ResizeMode = ResizeMode.NoResize;
                    completionWindow.BorderThickness = new Thickness(0);
                    IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                    foreach (var f in allTokens)
                    {
                        data.Add(CompletionDataSql.fromToken(f));
                    }
                    Debug.WriteLine($"completionWindow Show... ");

                    completionWindow.Show();
                    completionWindow.Closed += delegate {
                        Debug.WriteLine($"Closed... ");
                        completionWindow = null;
                        SetCompletionWindow(userSqliteTextEditor, null);
                    };
                    Debug.WriteLine($"StartOffset {completionWindow.StartOffset} -> {completionWindow.StartOffset - 1}, EndOffset:{completionWindow.EndOffset}.. ");
                    completionWindow.StartOffset -= 1;
                }
            }

            else
            {

            }



        }



        static void  textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            
            TextArea textArea = sender as TextArea;
            TextEditor userSqliteTextEditor = textArea == null ? null : GetTextEditor(textArea);
            if (userSqliteTextEditor == null) return;

            if (e.Text == "\n")
            {
                if (GetAcceptReturn(userSqliteTextEditor)==false)
                {
                    e.Handled = true;
                    return;
                }
            }

            CompletionWindow completionWindow = GetCompletionWindow(sender as DependencyObject);

            Debug.WriteLine($"-- Strt: {completionWindow?.StartOffset}");
            Debug.WriteLine($"End    : {completionWindow?.EndOffset}");
            Debug.WriteLine($"txt    : {e.Text}");
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (completionWindow.StartOffset == completionWindow.EndOffset)
                {
                    completionWindow.Close();
                    return;
                }
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    Debug.WriteLine($" RequestInsertion..");
                    completionWindow.CompletionList.RequestInsertion(e);
                    Debug.Write($" RequestInsertion e,completionWindow is {(completionWindow == null ? "" : "not ")}null");
                    Debug.Write($" Strt: {completionWindow?.StartOffset}");
                    Debug.WriteLine($" End : {completionWindow?.EndOffset}");
                }
            }
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }
        #endregion te_events_handlers




    }
}
