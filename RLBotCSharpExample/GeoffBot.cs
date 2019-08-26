using rlbot.flat;
using RLBotDotNet;
using System;

namespace Geoff {
    // We want to our bot to derive from Bot, and then implement its abstract methods.
    class GeoffBot : Bot
    {
        // We want the constructor for ExampleBot to extend from Bot, but we don't want to add anything to it.
        public GeoffBot(string botName, int botTeam, int botIndex) : base(botName, botTeam, botIndex) { }

        public override Controller GetOutput(GameTickPacket gameTickPacket)
        {
            // This controller object will be returned at the end of the method.
            // This controller will contain all the inputs that we want the bot to perform.
            Controller controller = new Controller();

            // Wrap gameTickPacket retrieving in a try-catch so that the bot doesn't crash whenever a value isn't present.
            // A value may not be present if it was not sent.
            // These are nullables so trying to get them when they're null will cause errors, therefore we wrap in try-catch.
            try
            {
                // Store the required data from the gameTickPacket.
                Vector3 ballLocation = gameTickPacket.Ball.Value.Physics.Value.Location.Value;
                Vector3 carLocation = gameTickPacket.Players(this.index).Value.Physics.Value.Location.Value;
                Rotator carRotation = gameTickPacket.Players(this.index).Value.Physics.Value.Rotation.Value;

                // Calculate to get the angle from the front of the bot's car to the ball.
                double botToTargetAngle = GetHorizontalAngle(carLocation, ballLocation);
                double botFrontToTargetAngle = botToTargetAngle - carRotation.Yaw;
                // Correct the angle
                botFrontToTargetAngle = CorrectAngle(botFrontToTargetAngle);

                // Decide which way to steer in order to get to the ball.
                controller.Steer = (float) (botFrontToTargetAngle / Math.PI);

                /* Throttle between 0.5 and 1 depending on the steering angle
                 * 0 Steer = 1 Throttle
                 * +-1 Steer = 0.5 Throttle
                 * A simply y = -2x+1 equation
                 */
                controller.Throttle = (float) (-2 * Math.Abs(controller.Steer)) + 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            return controller;
        }

        private double CorrectAngle(double angle) {
            if (angle < -Math.PI) {
                angle += 2 * Math.PI;
            } else if (angle > Math.PI) {
                angle -= 2 * Math.PI;
            }

            return angle;
        }
        
        private double GetHorizontalAngle(Vector3 from, Vector3 to) {
            return Math.Atan2(to.Y - from.Y, to.X - from.X);
        }

        private double GetVerticalAngle(Vector3 from, Vector3 to) {
            return Math.Atan2(to.Z - from.Z, to.X - from.X);
        }
    }
}
