﻿using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace QuakeConsole.Utilities
{
    /// <remarks>
    /// <see cref="IEnumerable" /> is implemented only to allow collection initializer syntax.
    /// </remarks>
    internal class ConsoleActionMap : IEnumerable
    {
        private readonly BiDirectionalDictionary<Int3, ConsoleAction> _map = new BiDirectionalDictionary<Int3, ConsoleAction>();

        private InputManager _input;

        public void LoadContent(Console console)
        {
            _input = console.ConsoleInput.Input;
        }

        public void Add(Keys modifier1, Keys modifier2, Keys key, ConsoleAction action)
        {
            _map.Add(new Int3((int)modifier1, (int)modifier2, (int)key), action);
        }

        public void Add(Keys modifier, Keys key, ConsoleAction action)
        {
            _map.Add(new Int3((int)Keys.None, (int) modifier, (int) key), action);
        }

        public void Add(Keys key, ConsoleAction action)
        {
            _map.Add(new Int3((int)Keys.None, (int) Keys.None, (int) key), action);
        }

        public bool AreModifiersAppliedForAction(ConsoleAction action)
        {
            bool modifiersAccepted = false;
            List<Int3> requiredModifiers;
            if (_map.BackwardTryGetValues(action, out requiredModifiers))
            {                
                foreach (Int3 modifiers in requiredModifiers)
                {
                    var modifier1 = (Keys)modifiers.X;
                    var modifier2 = (Keys)modifiers.Y;

                    modifiersAccepted = modifiersAccepted ||
                        (modifier1 == Keys.None || _input.IsKeyDown(modifier1)) &&
                        (modifier2 == Keys.None || _input.IsKeyDown(modifier2));
                }
            }
            return modifiersAccepted;
        }

        public bool TryGetAction(out ConsoleAction action)
        {                        
            foreach (Keys key in _input.PressedKeys)
            {
                // First look for actions with two modifiers.
                foreach (Keys modifier1 in _input.DownKeys)
                    foreach (Keys modifier2 in _input.DownKeys)
                    if (_map.ForwardTryGetValue(new Int3((int)modifier1, (int)modifier2, (int)key), out action))
                        return true;

                // Then look for actions with one modifier.
                foreach (Keys modifier in _input.DownKeys)
                    if (_map.ForwardTryGetValue(new Int3((int)Keys.None, (int)modifier, (int)key), out action))
                        return true;
            }
                
            // If not found; look for actions without modifiers.
            foreach (Keys key in _input.PressedKeys)
                if (_map.ForwardTryGetValue(new Int3((int)Keys.None, (int)Keys.None, (int)key), out action))
                    return true;

            action = default(ConsoleAction);
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
