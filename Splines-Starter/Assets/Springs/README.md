# Springs — starter

## Import the package

1. Open your road project, or any Unity 6 project made from the **Universal 3D** template.
2. **Assets → Import Package → Custom Package…**, choose `Springs-Starter.unitypackage`, and import everything. It all goes into `Assets/Springs/`.
3. Open a scene with text, such as `1. Basic`. When Unity asks, click **Import TMP Essentials**. If it does not ask and the text is missing, use **Window → TextMeshPro → Import TMP Essential Resources**, then open the scene again.

Module 3 in Unity **6000.6.5f1**. Seven scenes, one spring. We build scenes 1 to 4 together in class; you build scenes 5 to 7 on your own.

## Open the scenes

The scenes are in `Assets/Springs/Scenes/`. Each one is already set up with its art, UI, camera and wiring. The scenes play from the start, but nothing springs until you fill in the `TODO` comments.

| Scene | Built | Fill in | Try it |
|---|---|---|---|
| 1. Basic | in class | `TestBasicSpring` | Press Play. For another start, move the **Sphere** before Play |
| 2. Better | in class | `TestBetterSpring` | Same as scene 1, then change the damping ratio and the mass |
| 3. Juicy Slider | in class | `FloatSpring`, `JuicySlider` | Click or drag the slider |
| 4. Juicy Button | in class | `JuicyButton` | Press and hold the button. **Nudge** kicks it |
| 5. Juicier Slider | on your own | `JuicierSlider` | Drag the handle and let go |
| 6. Scale Slider | on your own | `ScaleSliderDemo` | Drag the scale slider |
| 7. Follow Rotation | on your own | `FollowRotationDemo` | Drag the target slider or the head |

Until you write `JuicySlider`, the scene 3 slider works like a plain Unity slider.

## The course spring

`FloatSpring` is the spring we write in class. Every scene from 3 on uses it, tuned by **frequency** (Hz) and **damping ratio**. Scenes 1 and 2 build the same math step by step on a single object.

The spring steps at a fixed 1/120 s, however long the frame was, so one slow frame cannot make it explode. `FloatSpring.Update(deltaTime)`, which you write, takes one step. `Advance(Time.deltaTime)` is supplied and calls `Update` in 1/120 s steps: call `Advance` once a frame. Scenes 1 and 2 do the same in their supplied `Update`.

## Follow the code

Scripts are in `Assets/Springs/Scripts/`. Each one you fill in has `TODO` comments saying what the step must do. The rest is supplied: the coil and overshoot markers in scenes 1 and 2, the mouse raycasts, the slot spacing in scene 5, the track mesh, and the dot, line and readouts in scene 7. Keep the public field names: the scenes are wired to them.

| Script | Supplied | You write |
|---|---|---|
| `JuicierSlider` | pressing, `PointerXOnTrack`, `NearestSlot` | the handle and press springs, snapping on release, moving the handle |
| `ScaleSliderDemo` | fields | the scale and the three readouts |
| `FollowRotationDemo` | dot, line, colours, readouts | the follow spring, moving the head, leaning it by velocity |

## Tests

Open **Window → General → Test Runner → EditMode**. `FloatSpringTests` has 6 tests of `FloatSpring`'s math. They fail until we write `FloatSpring` in class. They check the math only, not the feel: judge the feel by playing the scenes.
