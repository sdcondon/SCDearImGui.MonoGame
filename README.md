# SCDearImGui.MonoGame

Just my adaptation of the MonoGame ImGui renderer and examples found in the [ImGuiNET](https://github.com/ImGuiNET/ImGui.NET) monogame demo project.
Mostly for personal use, but making it public because others might find some value here - especially in the extensive demo project.

Changes from MonoGame demo proj in ImGuiNET:

* Significant changes made to the renderer from that project, which IMO leaves a lot to be desired. Most notably:
  * Don't do everything in Draw() - keep updates and draws separate. Firstly, in general it's a good idea to respect
    the conventions of the framework you are using - which in MonoGame's case means separating logic that updates
    state (which in ImGui's case happens alongside submitting GUI elements), and logic that sends to the graphics
    pipeline (which in ImGui's case doesn't happen until you retrieve the draw data and deal with it appropriately).
    More importantly, there are scenarios where its very useful to do updates and draws in different orders.
    For example, consider the overwhelmingly common scenario of one component being "on top" of another - it
    appears on top (easiest to achieve if its drawn last), and takes priority in capturing input (easiest to achieve
    if its updated first). There's an example of this in the demo project.
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
