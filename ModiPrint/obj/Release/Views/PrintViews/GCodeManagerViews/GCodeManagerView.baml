   M S B A M L   `   `   `  ���� c  _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity  D @ModiPrint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:ModiPrint	ModiPrint [2clr-namespace:ModiPrint.Utilities.IValueConverters$ModiPrint.Utilities.IValueConverters S OToggleSwitch, Version=1.1.2.0, Culture=neutral, PublicKeyToken=8637099990568f75A0clr-namespace:ToggleSwitch;assembly=ToggleSwitchToggleSwitch C   =ModiPrint.Views.PrintViews.GCodeManagerViews.GCodeManagerView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     )localclr-namespace:ModiPrint     @u2clr-namespace:ModiPrint.Utilities.IValueConverters     IToggleSwitch0clr-namespace:ToggleSwitch;assembly=ToggleSwitch       � 	Resources      BooleanToVisibilityConverter��%   &	        �� .���  � RowDefinitions �� $
��Auto ��� $��* �.+�� $��0��.+���  �� Orientation$ Vertical=�.+��� $ 
Horizontal=�.+���   ThematicButtonBorder#���  $
��Left=�._���   ThematicButton#���    StandardImageButtonHeight#���  ���� *UploadGCodeFileCommand+���� .R��� $ 
Horizontal=�
������ .+��� $'��!/Resources/Buttons/UploadFile.png��#���    ThematicControlImage#���  $��
0, 0, 0, 0q��� $ Vertical=�$��Center=�$��
5, 5, 5, 5q�.+��� $��Upload��  ThematicTextBlock#���  $��
0, 0, 0, 0q��� $��G-code��#���  $��
0, 0, 0, 0q� � ToolTip ��   ThematicToolTipTextBlock#���  %#Upload a RepRap flavor .gcode file.�� OMBefore printing, set the appropriate print parameters then hit Convert GCode.�� $ Vertical=�.+��� $��Center=�$��30��$	��500��#���  ���� *GCodeFileName+�� $��Center=�$��30��$	��500��#���  ���� *GCodeConverterStatus+�� #���  $��Right=�._��� #���  #���  ���� *SaveModiPrintGCodeFileCommand+���� .R��� $ 
Horizontal=�
������ .+��� $%��/Resources/Buttons/SaveFile.png��#���  #���  $��
0, 0, 0, 0q��� $ Vertical=�$��Center=�$��
5, 5, 5, 5q�.+��� $
��Save��#���  $��
0, 0, 0, 0q��� $��G-code��#���  $��
0, 0, 0, 0q� �� #���  20Save the converted GCode file into a .mdpt file.�� dbRequires a .gcode file to be uploaded then converted or a converted Gcode file to be loaded first.