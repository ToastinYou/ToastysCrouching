﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace Client
{
    public class Main : BaseScript
    {
        private Ped _p = Game.PlayerPed;

        // LEFT CTRL
        private Control _crControl = Control.Duck;

        private bool _crouched = false;
        private string _animSetCrouch = "move_ped_crouched";

        public Main()
        {
            RequestAnimSet();
            RegisterCommands();

            Tick += HandleControls;
        }

        private void ToggleCrouch()
        {
            if (_p.IsOnFoot == true && _p.IsDead == false && Game.IsPaused == false &&
                API.IsPedInAnyVehicle(_p.Handle, true) == false)
            {
                _crouched = !_crouched;

                // make ped crouch.
                if (_crouched == true)
                {
                    if (API.HasAnimSetLoaded(_animSetCrouch) == false)
                    {
                        API.RequestAnimSet(_animSetCrouch);
                    }

                    API.SetPedMovementClipset(_p.Handle, _animSetCrouch, 1.0f);
                }
                else // undo the crouching.
                {
                    API.ResetPedMovementClipset(_p.Handle, 0.0f);
                    _p.Task.ClearAll();
                }
            }
        }

        private async void RequestAnimSet()
        {
            API.RequestAnimSet(_animSetCrouch);

            while (API.HasAnimSetLoaded(_animSetCrouch) == false)
            {
                await Delay(100);
            }
        }

        private void RegisterCommands()
        {
            Action<int, List<object>, string> handleCommand = new Action<int, List<object>, string>((source, args, raw) =>
            {
                // Handle code in here for _any_ command.

                Debug.Write($"{source} - {args} - {raw}");

                if (raw != null)
                {
                    string msg = raw.ToLower();

                    if (msg == "crouch" || msg == "cr")
                    {
                        ToggleCrouch();
                    }
                }
            });

            string[] commands = new string[] { "crouch", "cr" };

            foreach (string cmd in commands)
            {
                API.RegisterCommand(cmd, handleCommand, false);
            }
        }

        private async Task HandleControls()
        {
            if (_p != null)
            {
                Game.DisableControlThisFrame(0, _crControl);

                if (Game.IsControlJustPressed(0, _crControl))
                {
                    ToggleCrouch();
                }
            }
            else
            {
                _p = Game.PlayerPed;
            }
        }
    }
}