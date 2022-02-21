# DCSpectatorCam

DCSpectatorCam is a simple BepInEx plugin for Audio Trip that modifies the default spectator camera and gives easy control over it with a config file or the VR controllers.

## Installing

1. Install [BepInEx](https://github.com/BepInEx/BepInEx) in the Audio Trip folder.
2. Unpack the DCSpectatorCam folder to the .\BepInEx\plugins\ folder.

## Using

1. Start Audio Trip and wait for the spectator camera to show up.
2. Focus on that window and press 'c' to enable the third-party camera.
3. Press 'e' to activate camera editing.
4. Set your cameras position by moving your right controller to where the camera is and pulling the trigger.
5. Step back into your play space and pull the trigger on the left controller to set the focal point.
6. Press 'e' to deavtivate camera editing.

### Notes

A config file is generated at .\BepinEx\config\DCSpectatorCam.cfg where you can edit all values.

You can also set the vertical FOV in here to match your real world camera for more accurate compositing with Mixed Reality.

I made sure this was done before setting the focal point to make it easier to get right the first time. (The default of 82.1° is a Logitech BRIO in portrait mode, if you were wondering.)

## Building

I threw this together with VS 2022 Community, the [BepInEx Plugin Templates](https://github.com/BepInEx/BepInEx.Templates), and a couple days of learning on the fly so don't expect it to be pretty.

Other libraries you'll need are .NET 4.6, Unity Engine, HarmonyX, and BepInEx.