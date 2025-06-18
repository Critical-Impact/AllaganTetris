namespace AllaganTetris.Windows;

using Dalamud.Interface.Windowing;
using ImGuiNET;

public class ConfigWindow(Configuration config) : Window("AllaganTetris Config")
{
    public override void Draw()
    {
        var configOption = config.ConfigOption;
        
        if (ImGui.Checkbox("Config Option", ref configOption))
        {
            config.ConfigOption = configOption;
        }
    }
}