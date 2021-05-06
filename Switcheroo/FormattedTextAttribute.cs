﻿// Copyright by Switcheroo

#region

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

#endregion

namespace Switcheroo {
    public class FormattedTextAttribute {
        public static readonly DependencyProperty FormattedTextProperty = DependencyProperty.RegisterAttached(
            "FormattedText",
            typeof(string),
            typeof(FormattedTextAttribute),
            new UIPropertyMetadata("", FormattedTextChanged));

        public static void SetFormattedText(DependencyObject textBlock, string value) {
            textBlock.SetValue(FormattedTextProperty, value);
        }

        private static void FormattedTextChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) {
            if (dependencyObject is not TextBlock textBlock) return;

            string formattedText = (string)dependencyPropertyChangedEventArgs.NewValue ?? string.Empty;
            formattedText =
                @"<Span xml:space=""preserve"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">" +
                formattedText +
                "</Span>";

            textBlock.Inlines.Clear();
            Span result = (Span)XamlReader.Parse(formattedText);
            textBlock.Inlines.Add(result);
        }
    }
}