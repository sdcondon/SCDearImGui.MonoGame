# SCDearImGui.MonoGame

Just my adaptation of the MonoGame ImGui renderer and examples found in the [ImGuiNET](https://github.com/ImGuiNET/ImGui.NET) monogame demo project.
Mostly for personal use, but making it public because others might find some value here - especially in the extensive demo project.

Changes from MonoGame demo proj in ImGuiNET:

* Significant changes made to the renderer from that project, which IMO leaves a lot to be desired. Most notably:
  * While I understand the temptation given ImGui's immediate nature, don't do everything in Draw(). MonoGame
    doesn't necessarily do a Draw for every update step (see its game loop documentation), and the last thing
    we want if the game is struggling to keep up is for our GUI to become even less responsive because button
    clicks etc aren't coinciding with a Draw call. Plus of course in general it's a good idea to respect the
    conventions of the framework you are using - which in MonoGame's case means separating logic that updates
    state (which in ImGui's case happens alongside submitting GUI elements), and logic that sends to the graphics
    pipeline (which in ImGui's case doesn't happen until you retrieve the draw data and deal with it appropriately).
  * Key up/down event code rewritten, because enumerating a fairly large enumeration in each update is slightly 
    insane, when instead we can just use MonoGame's GetPressedKeys stuff - which does bitwise operations to look
    for pressed keys. Note the benchmarks proj in the solution - which proves that my way is significantly faster.
  * Richer functionality around font and style management - to allow for easily scaling the GUI. Font atlas rebuild
    method replaced with a method for storing a reference style for scaling, a method for registering a font, and a method
    for applying a particular scale to the GUI and fonts.
  * Support for respecting and updating broader input capture state - not using input if it has already been consumed
    by something else, and informing other components that the GUI has captured input.
* Extensive demos. Started by rewriting those found 
  [here](https://github.com/tsMezotic/MonoGame.ImGuiNet/blob/main/Monogame.ImGuiNetSamples/Game1.cs) (themselves ported from
  the native examples) for better encapsulation and general code cleanliness, and added a several more - some others ported from
  the native examples, and some (an example display settings window, rendering a 3D model to a window,..) from scratch.
