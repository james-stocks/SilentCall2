using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

//19/09/2011 - James Stocks - jstocks@gmail.com
//Controller - represents a 360 gamepad
//Controller.Update() must be called at the end of the game's Update()

namespace Amadues
{
    class Controller
    {
        float LEFT_STICK_SENSITIVITY = 0.9f;
        double RIGHT_STICK_SENSITIVITY = 0.5;
        double TRIGGER_THRESHOLD = 0.5;
        double LEFT_STICK_DIAGONAL_SENSITIVITY = 0.4;
        float controllerVibrate = 0f;
        float controllerVibrateDropOff = 0.95f;

        bool movingWithDPad = false;

        PlayerIndex playerIndex;
        bool playerIndexSet = false;
        GamePadState lastButtons;
        int leftStickHoldDirection = SC.DIRECTION_NONE;
        int leftStickHoldDuration = 0;

        public Controller()
        {

        }

        public void Update()
        {
            if (playerIndexSet) lastButtons = GamePad.GetState(playerIndex);
        }

        public void SetPlayerIndex(PlayerIndex aPI)
        {
            playerIndex = aPI;
            playerIndexSet = true;
        }

        public void SetVibration(float LeftMotor, float RightMotor)
        {
            if (playerIndexSet) GamePad.SetVibration(playerIndex, LeftMotor, RightMotor);
        }

        public bool MovingWithDPad()
        {
            return movingWithDPad;
        }

        public void SetMovingWithDPad(bool aBool)
        {
            movingWithDPad = aBool;
        }

        public bool AIsPressed()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            return (GamePad.GetState(playerIndex).Buttons.A == ButtonState.Pressed && lastButtons.Buttons.A == ButtonState.Released);
        }
        public bool AIsHeld()
        {
            if (!playerIndexSet) return false;
            return GamePad.GetState(playerIndex).Buttons.A == ButtonState.Pressed;
        }

        public bool BIsPressed()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            return (GamePad.GetState(playerIndex).Buttons.B == ButtonState.Pressed && lastButtons.Buttons.B == ButtonState.Released);
        }
        public bool BIsHeld()
        {
            if (!playerIndexSet) return false;
            return GamePad.GetState(playerIndex).Buttons.B == ButtonState.Pressed;
        }

        public bool XIsPressed()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            return (GamePad.GetState(playerIndex).Buttons.X == ButtonState.Pressed && lastButtons.Buttons.X == ButtonState.Released);
        }
        public bool XIsHeld()
        {
            if (!playerIndexSet) return false;
            return GamePad.GetState(playerIndex).Buttons.X == ButtonState.Pressed;
        }

        public bool YIsPressed()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            return (GamePad.GetState(playerIndex).Buttons.Y == ButtonState.Pressed && lastButtons.Buttons.Y == ButtonState.Released);
        }
        public bool YIsHeld()
        {
            if (!playerIndexSet) return false;
            return GamePad.GetState(playerIndex).Buttons.Y == ButtonState.Pressed;
        }

        public bool BackIsPressed()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            return (GamePad.GetState(playerIndex).Buttons.Back == ButtonState.Pressed && lastButtons.Buttons.Back == ButtonState.Released);
        }
        public bool BackIsHeld()
        {
            if (!playerIndexSet) return false;
            return GamePad.GetState(playerIndex).Buttons.Back == ButtonState.Pressed;
        }
        public bool BackIsReleased()
        {
            return GamePad.GetState(playerIndex).Buttons.Back == ButtonState.Released;
        }

        public bool StartIsPressed()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            return (GamePad.GetState(playerIndex).Buttons.Start == ButtonState.Pressed && lastButtons.Buttons.Start == ButtonState.Released);
        }
        public bool StartIsHeld()
        {
            if (!playerIndexSet) return false;
            return GamePad.GetState(playerIndex).Buttons.Start == ButtonState.Pressed;
        }

        public bool LeftShoulderIsPressed()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            return GamePad.GetState(playerIndex).Buttons.LeftShoulder == ButtonState.Pressed
                && lastButtons.Buttons.LeftShoulder == ButtonState.Released;
        }
        public bool LeftShoulderIsHeld()
        {
            if (!playerIndexSet) return false;
            return GamePad.GetState(playerIndex).Buttons.LeftShoulder == ButtonState.Pressed;
        }
        public bool LeftShoulderIsReleased()
        {
            if (!playerIndexSet) return false;
            return GamePad.GetState(playerIndex).Buttons.LeftShoulder == ButtonState.Released;
        }

        public bool RightShoulderIsPressed()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            return GamePad.GetState(playerIndex).Buttons.RightShoulder == ButtonState.Pressed
                && lastButtons.Buttons.RightShoulder == ButtonState.Released;
        }
        public bool RightShoulderIsHeld()
        {
            if (!playerIndexSet) return false;
            return GamePad.GetState(playerIndex).Buttons.RightShoulder == ButtonState.Pressed;
        }
        public bool RightShoulderIsReleased()
        {
            if (!playerIndexSet) return false;
            return GamePad.GetState(playerIndex).Buttons.RightShoulder == ButtonState.Released;
        }

        public bool LeftTriggerIsPulled()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            return GamePad.GetState(playerIndex).Triggers.Left > TRIGGER_THRESHOLD
                && lastButtons.Triggers.Left <= TRIGGER_THRESHOLD;
        }
        public bool LeftTriggerIsHeld()
        {
            if (!playerIndexSet) return false;
            return GamePad.GetState(playerIndex).Triggers.Left > TRIGGER_THRESHOLD;
        }

        public bool RightTriggerIsPulled()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            return GamePad.GetState(playerIndex).Triggers.Right > TRIGGER_THRESHOLD
                && lastButtons.Triggers.Right <= TRIGGER_THRESHOLD;
        }
        public bool RightTriggerIsHeld()
        {
            if (!playerIndexSet) return false;
            return GamePad.GetState(playerIndex).Triggers.Right > TRIGGER_THRESHOLD;
        }

        public bool DownIsPressed()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            if (movingWithDPad)
            {
                return GamePad.GetState(playerIndex).DPad.Down == ButtonState.Pressed
                    && lastButtons.DPad.Down == ButtonState.Released;
            }
            else
            {
                return GamePad.GetState(playerIndex).ThumbSticks.Left.Y < -LEFT_STICK_SENSITIVITY
                    && lastButtons.ThumbSticks.Left.Y >= -LEFT_STICK_SENSITIVITY;
            }
        }
        public bool DownIsHeld()
        {
            if (!playerIndexSet) return false;
            if (movingWithDPad)
            {
                return GamePad.GetState(playerIndex).DPad.Down == ButtonState.Pressed;
            }
            else
            {
                return GamePad.GetState(playerIndex).ThumbSticks.Left.Y < -LEFT_STICK_SENSITIVITY;
            }
        }
        public bool DownIsReleased()
        {
            if (!playerIndexSet) return false;
            if (movingWithDPad)
            {
                return GamePad.GetState(playerIndex).DPad.Down == ButtonState.Released;
            }
            else
            {
                return GamePad.GetState(playerIndex).ThumbSticks.Left.Y >= -LEFT_STICK_SENSITIVITY;
            }
        }
        public bool LogDownIsPressed()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            if (!movingWithDPad)
            {
                return GamePad.GetState(playerIndex).DPad.Down == ButtonState.Pressed
                    && lastButtons.DPad.Down == ButtonState.Released;
            }
            else
            {
                return GamePad.GetState(playerIndex).ThumbSticks.Left.Y < -LEFT_STICK_SENSITIVITY
                    && lastButtons.ThumbSticks.Left.Y >= -LEFT_STICK_SENSITIVITY;
            }
        }
        public bool MenuDownIsPressed()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            if (GamePad.GetState(playerIndex).DPad.Down == ButtonState.Pressed
                    && lastButtons.DPad.Down == ButtonState.Released) return true;
            return GamePad.GetState(playerIndex).ThumbSticks.Left.Y < -LEFT_STICK_SENSITIVITY
                    && lastButtons.ThumbSticks.Left.Y >= -LEFT_STICK_SENSITIVITY;
        }
        public bool LastDownReleased()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            if (movingWithDPad)
            {
                return lastButtons.DPad.Down == ButtonState.Released;
            }
            else
            {
                return lastButtons.ThumbSticks.Left.Y >= -LEFT_STICK_SENSITIVITY;
            }
        }

        public bool UpIsPressed()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            if (movingWithDPad)
            {
                return GamePad.GetState(playerIndex).DPad.Up == ButtonState.Pressed
                    && lastButtons.DPad.Up == ButtonState.Released;
            }
            else
            {
                return GamePad.GetState(playerIndex).ThumbSticks.Left.Y > LEFT_STICK_SENSITIVITY
                    && lastButtons.ThumbSticks.Left.Y <= LEFT_STICK_SENSITIVITY;
            }
        }
        public bool UpIsHeld()
        {
            if (!playerIndexSet) return false;
            if (movingWithDPad)
            {
                return GamePad.GetState(playerIndex).DPad.Up == ButtonState.Pressed;
            }
            else
            {
                return GamePad.GetState(playerIndex).ThumbSticks.Left.Y > LEFT_STICK_SENSITIVITY;
            }
        }
        public bool UpIsReleased()
        {
            if (!playerIndexSet) return false;
            if (movingWithDPad)
            {
                return GamePad.GetState(playerIndex).DPad.Up == ButtonState.Released;
            }
            else
            {
                return GamePad.GetState(playerIndex).ThumbSticks.Left.Y <= LEFT_STICK_SENSITIVITY;
            }
        }
        public bool LogUpIsPressed()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            if (!movingWithDPad)
            {
                return GamePad.GetState(playerIndex).DPad.Up == ButtonState.Pressed
                    && lastButtons.DPad.Up == ButtonState.Released;
            }
            else
            {
                return GamePad.GetState(playerIndex).ThumbSticks.Left.Y > LEFT_STICK_SENSITIVITY
                    && lastButtons.ThumbSticks.Left.Y <= LEFT_STICK_SENSITIVITY;
            }
        }
        public bool MenuUpIsPressed()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            if (GamePad.GetState(playerIndex).DPad.Up == ButtonState.Pressed
                    && lastButtons.DPad.Up == ButtonState.Released) return true;
            return GamePad.GetState(playerIndex).ThumbSticks.Left.Y > LEFT_STICK_SENSITIVITY
                    && lastButtons.ThumbSticks.Left.Y <= LEFT_STICK_SENSITIVITY;
        }
        public bool LastUpReleased()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            if (movingWithDPad)
            {
                return lastButtons.DPad.Up == ButtonState.Released;
            }
            else
            {
                return lastButtons.ThumbSticks.Left.Y <= LEFT_STICK_SENSITIVITY;
            }
        }

        public bool LeftIsPressed()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            if (movingWithDPad)
            {
                return GamePad.GetState(playerIndex).DPad.Left == ButtonState.Pressed
                    && lastButtons.DPad.Left == ButtonState.Released;
            }
            else
            {
                return GamePad.GetState(playerIndex).ThumbSticks.Left.X < -LEFT_STICK_SENSITIVITY
                    && lastButtons.ThumbSticks.Left.X >= -LEFT_STICK_SENSITIVITY;
            }
        }
        public bool MenuLeftIsPressed()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            if (GamePad.GetState(playerIndex).DPad.Left == ButtonState.Pressed
                    && lastButtons.DPad.Left == ButtonState.Released) return true;
            return GamePad.GetState(playerIndex).ThumbSticks.Left.X < -LEFT_STICK_SENSITIVITY
                    && lastButtons.ThumbSticks.Left.X >= -LEFT_STICK_SENSITIVITY;
        }
        public bool LeftIsHeld()
        {
            if (!playerIndexSet) return false;
            if (movingWithDPad)
            {
                return GamePad.GetState(playerIndex).DPad.Left == ButtonState.Pressed;
            }
            else
            {
                return GamePad.GetState(playerIndex).ThumbSticks.Left.X < -LEFT_STICK_SENSITIVITY;
            }
        }
        public bool LeftIsReleased()
        {
            if (!playerIndexSet) return false;
            if (movingWithDPad)
            {
                return GamePad.GetState(playerIndex).DPad.Left == ButtonState.Released;
            }
            else
            {
                return GamePad.GetState(playerIndex).ThumbSticks.Left.X >= -LEFT_STICK_SENSITIVITY;
            }
        }
        public bool LastLeftReleased()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            if (movingWithDPad)
            {
                return lastButtons.DPad.Left == ButtonState.Released;
            }
            else
            {
                return lastButtons.ThumbSticks.Left.X >= -LEFT_STICK_SENSITIVITY;
            }
        }

        public bool RightIsPressed()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            if (movingWithDPad)
            {
                return GamePad.GetState(playerIndex).DPad.Right == ButtonState.Pressed
                    && lastButtons.DPad.Right == ButtonState.Released;
            }
            else
            {
                return GamePad.GetState(playerIndex).ThumbSticks.Left.X > LEFT_STICK_SENSITIVITY
                    && lastButtons.ThumbSticks.Left.X <= LEFT_STICK_SENSITIVITY;
            }
        }
        public bool MenuRightIsPressed()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            if (GamePad.GetState(playerIndex).DPad.Right == ButtonState.Pressed
                    && lastButtons.DPad.Right == ButtonState.Released) return true;
            return GamePad.GetState(playerIndex).ThumbSticks.Left.X > LEFT_STICK_SENSITIVITY
                   && lastButtons.ThumbSticks.Left.X <= LEFT_STICK_SENSITIVITY;
        }
        public bool RightIsHeld()
        {
            if (!playerIndexSet) return false;
            if (movingWithDPad)
            {
                return GamePad.GetState(playerIndex).DPad.Right == ButtonState.Pressed;
            }
            else
            {
                return GamePad.GetState(playerIndex).ThumbSticks.Left.X > LEFT_STICK_SENSITIVITY;
            }
        }
        public bool RightIsReleased()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            if (movingWithDPad)
            {
                return GamePad.GetState(playerIndex).DPad.Right == ButtonState.Released;
            }
            else
            {
                return GamePad.GetState(playerIndex).ThumbSticks.Left.X <= LEFT_STICK_SENSITIVITY;
            }
        }
        public bool LastRightReleased()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            if (movingWithDPad)
            {
                return lastButtons.DPad.Right == ButtonState.Released;
            }
            else
            {
                return lastButtons.ThumbSticks.Left.X <= LEFT_STICK_SENSITIVITY;
            }
        }

        public bool UpLeftDiagonalHeld()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            if (movingWithDPad)
            {
                return GamePad.GetState(playerIndex).DPad.Up == ButtonState.Pressed
                    && GamePad.GetState(playerIndex).DPad.Left == ButtonState.Pressed;
            }
            else
            {
                return GamePad.GetState(playerIndex).ThumbSticks.Left.X < -LEFT_STICK_DIAGONAL_SENSITIVITY
                    && GamePad.GetState(playerIndex).ThumbSticks.Left.Y > LEFT_STICK_DIAGONAL_SENSITIVITY;
            }
        }
        public bool UpLeftDiagonalPressed()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            if (movingWithDPad)
            {
                return (GamePad.GetState(playerIndex).DPad.Up == ButtonState.Pressed
                    && GamePad.GetState(playerIndex).DPad.Left == ButtonState.Pressed) 
                    && (lastButtons.DPad.Up == ButtonState.Released || lastButtons.DPad.Left == ButtonState.Released);
            }
            else
            {
                return (GamePad.GetState(playerIndex).ThumbSticks.Left.X < -LEFT_STICK_DIAGONAL_SENSITIVITY
                    && GamePad.GetState(playerIndex).ThumbSticks.Left.Y > LEFT_STICK_DIAGONAL_SENSITIVITY)
                    && (lastButtons.ThumbSticks.Left.X >= -LEFT_STICK_DIAGONAL_SENSITIVITY
                        || lastButtons.ThumbSticks.Left.Y <= LEFT_STICK_DIAGONAL_SENSITIVITY);
            }
        }

        public bool UpRightDiagonalHeld()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            if (movingWithDPad)
            {
                return GamePad.GetState(playerIndex).DPad.Up == ButtonState.Pressed
                    && GamePad.GetState(playerIndex).DPad.Right == ButtonState.Pressed;
            }
            else
            {
                return GamePad.GetState(playerIndex).ThumbSticks.Left.X > LEFT_STICK_DIAGONAL_SENSITIVITY
                    && GamePad.GetState(playerIndex).ThumbSticks.Left.Y > LEFT_STICK_DIAGONAL_SENSITIVITY;
            }
        }
        public bool UpRightDiagonalPressed()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            if (movingWithDPad)
            {
                return GamePad.GetState(playerIndex).DPad.Up == ButtonState.Pressed
                    && GamePad.GetState(playerIndex).DPad.Right == ButtonState.Pressed
                    && (lastButtons.DPad.Up == ButtonState.Released 
                        || lastButtons.DPad.Right == ButtonState.Released);
            }
            else
            {
                return GamePad.GetState(playerIndex).ThumbSticks.Left.X > LEFT_STICK_DIAGONAL_SENSITIVITY
                    && GamePad.GetState(playerIndex).ThumbSticks.Left.Y > LEFT_STICK_DIAGONAL_SENSITIVITY
                    && (lastButtons.ThumbSticks.Left.X <= LEFT_STICK_DIAGONAL_SENSITIVITY
                        || lastButtons.ThumbSticks.Left.Y <= LEFT_STICK_DIAGONAL_SENSITIVITY);
            }
        }

        public bool DownLeftDiagonalHeld()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            if (movingWithDPad)
            {
                return GamePad.GetState(playerIndex).DPad.Down == ButtonState.Pressed
                    && GamePad.GetState(playerIndex).DPad.Left == ButtonState.Pressed;
            }
            else
            {
                return GamePad.GetState(playerIndex).ThumbSticks.Left.X < -LEFT_STICK_DIAGONAL_SENSITIVITY
                    && GamePad.GetState(playerIndex).ThumbSticks.Left.Y < -LEFT_STICK_DIAGONAL_SENSITIVITY;
            }
        }
        public bool DownLeftDiagonalPressed()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            if (movingWithDPad)
            {
                return GamePad.GetState(playerIndex).DPad.Down == ButtonState.Pressed
                    && GamePad.GetState(playerIndex).DPad.Left == ButtonState.Pressed
                    && (lastButtons.DPad.Down == ButtonState.Released 
                        || lastButtons.DPad.Left == ButtonState.Released);
            }
            else
            {
                return GamePad.GetState(playerIndex).ThumbSticks.Left.X < -LEFT_STICK_DIAGONAL_SENSITIVITY
                    && GamePad.GetState(playerIndex).ThumbSticks.Left.Y < -LEFT_STICK_DIAGONAL_SENSITIVITY
                    && (lastButtons.ThumbSticks.Left.X >= -LEFT_STICK_DIAGONAL_SENSITIVITY
                        || lastButtons.ThumbSticks.Left.Y >= -LEFT_STICK_DIAGONAL_SENSITIVITY);
            }
        }

        public bool DownRightDiagonalHeld()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            if (movingWithDPad)
            {
                return GamePad.GetState(playerIndex).DPad.Down == ButtonState.Pressed
                    && GamePad.GetState(playerIndex).DPad.Right == ButtonState.Pressed;
            }
            else
            {
                return GamePad.GetState(playerIndex).ThumbSticks.Left.X > LEFT_STICK_DIAGONAL_SENSITIVITY
                    && GamePad.GetState(playerIndex).ThumbSticks.Left.Y < -LEFT_STICK_DIAGONAL_SENSITIVITY;
            }
        }
        public bool DownRightDiagonalPressed()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            if (movingWithDPad)
            {
                return GamePad.GetState(playerIndex).DPad.Down == ButtonState.Pressed
                    && GamePad.GetState(playerIndex).DPad.Right == ButtonState.Pressed
                    && (lastButtons.DPad.Down == ButtonState.Released
                        || lastButtons.DPad.Right == ButtonState.Released);
            }
            else
            {
                return GamePad.GetState(playerIndex).ThumbSticks.Left.X > LEFT_STICK_DIAGONAL_SENSITIVITY
                    && GamePad.GetState(playerIndex).ThumbSticks.Left.Y < -LEFT_STICK_DIAGONAL_SENSITIVITY
                    && (lastButtons.ThumbSticks.Left.X <= LEFT_STICK_DIAGONAL_SENSITIVITY
                        || lastButtons.ThumbSticks.Left.Y >= -LEFT_STICK_DIAGONAL_SENSITIVITY);
            }
        }

        public bool RightStickLeftHeld()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            return GamePad.GetState(playerIndex).ThumbSticks.Right.X < -RIGHT_STICK_SENSITIVITY;
        }
        public bool RightStickRightHeld()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            return GamePad.GetState(playerIndex).ThumbSticks.Right.X > RIGHT_STICK_SENSITIVITY;
        }
        public bool RightStickUpHeld()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            return GamePad.GetState(playerIndex).ThumbSticks.Right.Y > RIGHT_STICK_SENSITIVITY;
        }
        public bool RightStickDownHeld()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            return GamePad.GetState(playerIndex).ThumbSticks.Right.Y < -RIGHT_STICK_SENSITIVITY;
        }
        public bool RightStickUpLeftDiagonalHeld()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            return GamePad.GetState(playerIndex).ThumbSticks.Right.X < -LEFT_STICK_DIAGONAL_SENSITIVITY
                   && GamePad.GetState(playerIndex).ThumbSticks.Right.Y > LEFT_STICK_DIAGONAL_SENSITIVITY;
        }
        public bool RightStickUpRightDiagonalHeld()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            return GamePad.GetState(playerIndex).ThumbSticks.Right.X > LEFT_STICK_DIAGONAL_SENSITIVITY
                   && GamePad.GetState(playerIndex).ThumbSticks.Right.Y > LEFT_STICK_DIAGONAL_SENSITIVITY;
        }
        public bool RightStickDownLeftDiagonalHeld()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            return GamePad.GetState(playerIndex).ThumbSticks.Right.X < -LEFT_STICK_DIAGONAL_SENSITIVITY
                   && GamePad.GetState(playerIndex).ThumbSticks.Right.Y < -LEFT_STICK_DIAGONAL_SENSITIVITY;
        }
        public bool RightStickDownRightDiagonalHeld()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            return GamePad.GetState(playerIndex).ThumbSticks.Right.X > LEFT_STICK_DIAGONAL_SENSITIVITY
                   && GamePad.GetState(playerIndex).ThumbSticks.Right.Y < -LEFT_STICK_DIAGONAL_SENSITIVITY;
        }

        public bool RightStickAnyDirectionHeld()
        {
            if (!playerIndexSet) return false;
            if (lastButtons == null) return false;
            return GamePad.GetState(playerIndex).ThumbSticks.Right.X < -RIGHT_STICK_SENSITIVITY
                || GamePad.GetState(playerIndex).ThumbSticks.Right.X > RIGHT_STICK_SENSITIVITY
                || GamePad.GetState(playerIndex).ThumbSticks.Right.Y > RIGHT_STICK_SENSITIVITY
                || GamePad.GetState(playerIndex).ThumbSticks.Right.Y < -RIGHT_STICK_SENSITIVITY;
        }

    }
}
