﻿using System.Windows.Input;

namespace ArmA_3_Server_Tool
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand
            (
                "Exit",
                "Exit",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F4, ModifierKeys.Alt)
                }
            );

        public static readonly RoutedUICommand Settings = new RoutedUICommand
            (
                "Settings",
                "Settings",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.O, ModifierKeys.Alt)
                }
            );
        public static readonly RoutedUICommand CopyModNames = new RoutedUICommand
            (
                "Copy Mods names",
                "CopyModNames",
                typeof(CustomCommands)
            );

        public static readonly RoutedUICommand CopyModIds = new RoutedUICommand
            (
                "Copy Mods IDs",
                "CopyModIds",
                typeof(CustomCommands)
            );

        public static readonly RoutedUICommand CopyRegex = new RoutedUICommand
            (
                "Copy Regex",
                "CopyRegex",
                typeof(CustomCommands)
            );
    }
}
