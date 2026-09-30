using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static ImGuiNET.ImGui;
using static SCDearImGui.MonoGame.Demos.GuiElements.GuiElementHelpers;

namespace SCDearImGui.MonoGame.Demos.GuiElements.MiniApps;

class DocumentsWindow(ExampleDocumentStore documentStore, bool isOpen = false)
{
    public bool IsOpen = isOpen;

    private readonly ExampleDocumentStore documentStore = documentStore;
    private readonly List<Tab> tabs = [];

    private bool showDocumentStoreCheckboxes = false;

    public void Update()
    {
        if (!IsOpen) return;

        SetNextWindowSize(new(430, 450), ImGuiCond.FirstUseEver);
        if (Begin("Example: Documents", ref IsOpen, ImGuiWindowFlags.MenuBar))
        {
            UpdateMenuBar();
            UpdateDocumentStoreCheckboxes();
            UpdateTabs();
        }

        End();
    }

    private void UpdateMenuBar()
    {
        if (!BeginMenuBar()) return;

        if (BeginMenu("File"))
        {
            var documentMetadata = documentStore.GetDocumentMetadata().ToList();

            var openDocumentCount = documentMetadata.Count(d => IsDocumentOpen(d.id, out _));

            if (BeginMenu("Open", openDocumentCount < documentMetadata.Count))
            {
                foreach (var (id, name) in documentMetadata.Where(d => !IsDocumentOpen(d.id, out _)))
                {
                    if (MenuItem(name)) OpenDocument(id);
                }

                EndMenu();
            }

            if (MenuItem("Close All Documents", null, false, openDocumentCount > 0))
            {
                tabs.ForEach(t => t.IsClosing = true);
            }

            if (MenuItem("Exit"))
            {
                IsOpen = false;
            }

            EndMenu();
        }

        if (BeginMenu("Options"))
        {
            MenuItem("Show Document Store Checkboxes", null, ref showDocumentStoreCheckboxes);

            EndMenu();
        }

        EndMenuBar();
    }

    private void UpdateDocumentStoreCheckboxes()
    {
        if (!showDocumentStoreCheckboxes) return;

        Dummy(Vector2.Zero);
        foreach (var doc in documentStore.GetDocumentMetadata())
        {
            SameLine();
            PushID(doc.GetHashCode());
            var isOpen = IsDocumentOpen(doc.id, out var _);
            if (Checkbox(doc.name, ref isOpen))
            {
                if (isOpen)
                {
                    OpenDocument(doc.id);
                }
                else
                {
                    ForceCloseDocument(doc.id);
                }
            }
            PopID();
        }
    }

    private void UpdateTabs()
    {
        // An empty tab bar looks a bit ugly, so just omit it entirely.
        if (tabs.Count == 0) return;

        // Update the bar itself
        const ImGuiTabBarFlags tabBarFlags = ImGuiTabBarFlags.Reorderable
            | ImGuiTabBarFlags.FittingPolicyDefault
            | ImGuiTabBarFlags.DrawSelectedOverline
            | ImGuiTabBarFlags.AutoSelectNewTabs;
        if (BeginTabBar("##tabs", tabBarFlags))
        {
            foreach (var tab in tabs)
            {
                tab.Update();
            }

            EndTabBar();
        }

        // Finally, handle any tabs that are requesting closure
        var closingTabs = tabs.Where(t => t.IsClosing);
        var modifiedClosingTabs = closingTabs.Where(t => t.IsModified);
        if (modifiedClosingTabs.Any())
        {
            if (!IsPopupOpen("Save?"))
            {
                OpenPopup("Save?");
            }

            if (BeginPopupModal("Save?", ImGuiWindowFlags.AlwaysAutoResize))
            {
                Text("Save change to the following items?");

                if (BeginChild(GetID("frame"), new(-float.Epsilon, 6.25f * GetTextLineHeightWithSpacing()), ImGuiChildFlags.FrameStyle))
                    foreach (var tab in modifiedClosingTabs)
                        Text(tab.Document.Name);
                EndChild();

                var button_size = new Vector2(GetFontSize() * 7.0f, 0.0f);

                if (Button("Yes", button_size))
                {
                    foreach (var tab in modifiedClosingTabs)
                    {
                        tab.Save();
                    }
                    tabs.RemoveAll(t => t.IsClosing);
                    CloseCurrentPopup();
                }

                SameLine();
                if (Button("No", button_size))
                {
                    tabs.RemoveAll(t => t.IsClosing);
                    CloseCurrentPopup();
                }

                SameLine();
                if (Button("Cancel", button_size))
                {
                    foreach (var tab in closingTabs)
                    {
                        tab.IsClosing = false;
                    }
                    CloseCurrentPopup();
                }

                EndPopup();
            }
        }
        else
        {
            tabs.RemoveAll(t => t.IsClosing);
        }
    }

    private bool IsDocumentOpen(int documentId, out Tab tab)
    {
        tab = tabs.FirstOrDefault(t => t.Document.Id == documentId);
        return tab != null;
    }

    private bool OpenDocument(int documentId)
    {
        if (IsDocumentOpen(documentId, out _))
        {
            return false;
        }

        tabs.Add(new Tab(this, documentId));
        return true;
    }

    // todo: i think i might have missed the point here, and *when*
    // SetTabItemClosed is invoked is wrong. Check me.
    private bool ForceCloseDocument(int documentId)
    {
        if (!IsDocumentOpen(documentId, out var tab))
        {
            return false;
        }

        tabs.Remove(tab);

        // [Optional] Notify the system of Tabs/Windows closure that happened outside the regular tab interface.
        // If a tab has been closed programmatically (aka closed from another source such as the Checkbox() in the demo,
        // as opposed to clicking on the regular tab closing button) and stops being submitted, it will take a frame for
        // the tab bar to notice its absence. During this frame there will be a gap in the tab bar, and if the tab that has
        // disappeared was the selected one, the tab bar will report no selected tab during the frame. This will effectively
        // give the impression of a flicker for one frame.
        // We call SetTabItemClosed() to manually notify the Tab Bar or Docking system of removed tabs to avoid this glitch.
        // Note that this completely optional, and only affect tab bars with the ImGuiTabBarFlags_Reorderable flag.
        SetTabItemClosed(tab.Document.Name); // <-- is this right? doc id appended to tab name?

        return true;
    }

    private class Tab(DocumentsWindow parent, int documentId)
    {
        private string newDocumentName;

        public ExampleDocument Document { get; } = parent.documentStore.LoadDocument(documentId);

        public bool IsModified { get; private set; } = false;

        public bool IsClosing { get; set; } = false;

        public void Update()
        {
            PushID($"##tab-{documentId}");

            // As we allow to change document name, we append a never-changing document id so tabs are stable
            var tabName = $"{Document.Name}###doc{documentId}";
            // About the ImGuiWindowFlags_UnsavedDocument / ImGuiTabItemFlags_UnsavedDocument flags.
            // They have multiple effects:
            // - Display a dot next to the title.
            // - Tab is selected when clicking the X close button.
            // - Closure is not assumed (will wait for user to stop submitting the tab).
            //   Otherwise closure is assumed when pressing the X, so if you keep submitting the tab may reappear at end of tab bar.
            //   We need to assume closure by default otherwise waiting for "lack of submission" on the next frame would leave an empty
            //   hole for one-frame, both in the tab-bar and in tab-contents when closing a tab/window.
            //   The rarely used SetTabItemClosed() function is a way to notify of programmatic closure to avoid the one-frame hole.
            var tabItemFlags = IsModified ? ImGuiTabItemFlags.UnsavedDocument : 0;
            var shouldTabStayOpen = true;
            var isTabSelected = BeginTabItem(tabName, ref shouldTabStayOpen, tabItemFlags);
            if (!shouldTabStayOpen)
            {
                IsClosing = true;
            }

            var shouldOpenRenamingPopup = false;
            UpdateContextMenu(ref shouldOpenRenamingPopup);

            if (isTabSelected)
            {
                UpdateButtonBar(ref shouldOpenRenamingPopup);
                Separator();
                UpdateBody();
                EndTabItem();
            }

            UpdateRenamingPopup(shouldOpenRenamingPopup);
            PopID();
        }

        public void Save()
        {
            parent.documentStore.SaveDocument(Document);
            IsModified = false;
        }

        private void UpdateContextMenu(ref bool shouldOpenRenamingPopup)
        {
            if (BeginPopupContextItem())
            {
                if (MenuItem($"Save {Document.Name}", "Ctrl+S", false))
                {
                    Save();
                }

                if (MenuItem("Rename...", "Ctrl+R", false))
                {
                    shouldOpenRenamingPopup = true;
                }

                if (MenuItem("Close", "Ctrl+W", false))
                {
                    IsClosing = true;
                }

                EndPopup();
            }
        }

        private void UpdateButtonBar(ref bool shouldOpenRenamingPopup)
        {
            SetNextItemShortcut(ImGuiKey.ModCtrl | ImGuiKey.S, ImGuiInputFlags.Tooltip);
            if (Button("Save")) Save();

            SameLine();
            SetNextItemShortcut(ImGuiKey.ModCtrl | ImGuiKey.R, ImGuiInputFlags.Tooltip);
            if (Button("Rename")) shouldOpenRenamingPopup = true;

            SameLine();
            SetNextItemShortcut(ImGuiKey.ModCtrl | ImGuiKey.W, ImGuiInputFlags.Tooltip);
            if (Button("Close")) IsClosing = true;
        }

        private void UpdateBody()
        {
            Text($"Document \"{Document.Name}\"");

            Spacing();

            PushStyleColor(ImGuiCol.Text, Document.Color);
            if (InputTextMultiline("##content", ref Document.Content, 2048, new(GetContentRegionAvail().X, GetTextLineHeight() * 16)))
            {
                IsModified = true;
            }
            PopStyleColor();

            Spacing();

            // Useful to test drag and drop and hold-dragged-to-open-tab behavior.
            if (ColorEdit4("color", ref Document.Color))
            {
                IsModified = true;
            }
        }

        private void UpdateRenamingPopup(bool shouldOpenRenamingPopup)
        {
            if (shouldOpenRenamingPopup)
            {
                newDocumentName = Document.Name;
                OpenPopup("Rename");
            }

            if (BeginPopup("Rename"))
            {
                SetNextItemWidth(GetFontSize() * 30);
                if (InputText("###Name", ref newDocumentName, 64, ImGuiInputTextFlags.EnterReturnsTrue))
                {
                    IsModified = newDocumentName != Document.Name;
                    Document.Name = newDocumentName;
                    CloseCurrentPopup();
                    newDocumentName = null;
                }
                if (shouldOpenRenamingPopup)
                {
                    SetKeyboardFocusHere(-1);
                }

                EndPopup();
            }
        }
    }
}

// An example document store for the window to interact with, backed by just an in-memory list.
// In a real app, an external backing store is of course more likely.
// As such, it'd probably be useful if the methods here were async, and the window had logic
// to show "loading.." placeholders, deal with saving (and the cancellation thereof) gracefully,
// and handle metadata listing that can take a while (e.g. metadata to IAsyncEnumerable/Queryable).
// Perhaps later.
class ExampleDocumentStore
{
    private readonly List<ExampleDocument> content =
    [
        new ExampleDocument(0, "Icons",               "\u2623\u2623\u2623", new Vector4(0.4f, 0.8f, 0.4f, 1.0f)),
        new ExampleDocument(1, "Lettuce",             "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.", new Vector4(0.4f, 0.8f, 0.4f, 1.0f)),
        new ExampleDocument(2, "Eggplant",            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.", new Vector4(0.8f, 0.5f, 1.0f, 1.0f)),
        new ExampleDocument(3, "Carrot",              "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.", new Vector4(1.0f, 0.8f, 0.5f, 1.0f)),
        new ExampleDocument(4, "Tomato",              "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.", new Vector4(1.0f, 0.3f, 0.4f, 1.0f)),
        new ExampleDocument(5, "A Rather Long Title", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.", new Vector4(0.4f, 0.8f, 0.8f, 1.0f)),
        new ExampleDocument(6, "Some Document",       "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.", new Vector4(0.8f, 0.8f, 1.0f, 1.0f))
    ];

    public IEnumerable<(int id, string name)> GetDocumentMetadata()
    {
        foreach (var doc in content)
        {
            // todo: await Task.Delay(Random...)
            yield return (doc.Id, doc.Name);
        }
    }

    // todo: new documents - either a ExampleDocument NewDocument() method,
    // or allow null ID in doc model and handle in SaveDocument (probably the latter).

    public ExampleDocument LoadDocument(int documentId)
    {
        var storedDoc = content.Single(d => d.Id == documentId);

        var loadedDoc = new ExampleDocument(
            storedDoc.Id,
            storedDoc.Name,
            storedDoc.Content,
            storedDoc.Color);

        // todo: await Task.Delay(Random...)
        return loadedDoc;
    }

    public void SaveDocument(ExampleDocument document)
    {
        var storedDoc = content.Single(d => d.Id == document.Id);

        storedDoc.Name = document.Name;
        storedDoc.Content = document.Content;
        storedDoc.Color = document.Color;

        // todo: await Task.Delay(Random...)
    }
}

// An example document model for the window to work with.
class ExampleDocument(int id, string name, string content, Vector4 color)
{
    // A unique identifier for the document, on the assumption that the title is not
    // necessarily unique and/or can change. In a file-backed system this would likely
    // be a file path instead.
    public int Id = id;

    public string Name = name;

    public string Content = content;

    public Vector4 Color = color;
}