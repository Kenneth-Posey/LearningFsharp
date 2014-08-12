namespace Simulator

module Keyboard = 
    open System
    open System.Windows.Forms
    
    open WindowsInterop.DllImports
    open SimulatorTypes.Keyboard.Constants

    type Controller = 
        /// Passes parameters + default values to Dll wrapper
        static member private KeyEvent key code = 
            KeyboardControl.KeyboardEvent key 0uy code 0
            
        /// Transforms the alt/ctrl/shift key code to correct value
        static member ParseKey key =
            match key with
            | Keys.Alt     -> byte 18
            | Keys.Control -> byte 17
            | Keys.Shift   -> byte 16
            | _            -> byte key  // No need for transformation

        // Set keystate to down
        static member KeyDown key =
            Controller.KeyEvent <| Controller.ParseKey key, EventCode.KeyDown

        /// Set keystate to up
        static member KeyUp key = 
            Controller.KeyEvent <| Controller.ParseKey key, EventCode.KeyUp

        /// Presses and releases key
        static member KeyPress key =
            Controller.KeyDown key
            Controller.KeyUp   key

        /// Simulates the correct shortcut combination for each command
        static member SimulateShortcutCommand shortcut =
            // Static members can not be used in pattern matching
            let Close     = int StandardShortcut.Close
            let Copy      = int StandardShortcut.Copy
            let Cut       = int StandardShortcut.Cut      
            let Paste     = int StandardShortcut.Paste    
            let SelectAll = int StandardShortcut.SelectAll
            let Save      = int StandardShortcut.Save     
            let Open      = int StandardShortcut.Open     
            let New       = int StandardShortcut.New       
            let Print     = int StandardShortcut.Print    

            let ctrl key = 
                Controller.KeyDown  Keys.Control
                Controller.KeyPress key
                Controller.KeyUp    Keys.Control

            let alt key = 
                Controller.KeyDown  Keys.Alt
                Controller.KeyPress key
                Controller.KeyUp    Keys.Alt

            match shortcut with            
            | Close     -> alt  Keys.F4
            | Copy      -> ctrl Keys.C
            | Cut       -> ctrl Keys.X
            | Paste     -> ctrl Keys.P
            | SelectAll -> ctrl Keys.A
            | Save      -> ctrl Keys.S
            | Open      -> ctrl Keys.O
            | New       -> ctrl Keys.N
            | Print     -> ctrl Keys.P




        