   M S B A M L   `   `   `  ���� D  @ModiPrint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null�kclr-namespace:ModiPrint.Views.StatusAndControlViews.EquipmentSelectionViews.EquipmentSelectionSettingsViews]ModiPrint.Views.StatusAndControlViews.EquipmentSelectionViews.EquipmentSelectionSettingsViews  k    eModiPrint.Views.StatusAndControlViews.EquipmentSelectionViews.EquipmentSelectionSettingsViews.SetAxis   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     }localkclr-namespace:ModiPrint.Views.StatusAndControlViews.EquipmentSelectionViews.EquipmentSelectionSettingsViews     .��� $��Center=�.+���   �� Orientation$  Vertical=�.+��� $  
Horizontal=�$��Center=�.+��� $!��/Resources/General/Axis.png��$��Center=�   TallControlHeight#���   $��
5, 5, 5, 5q��� $  
Horizontal=�.+��� $��
Set Axis: ��  ThematicHeaderTextBlock#���  $��
5, 5, 0, 5q��� #���  $��
0, 5, 5, 5q����� *
AxisName+�� $��
Parameters��  ThematicGroupBox#���  .���� $  Vertical=�$��Center=�.+��� $  
Horizontal=�  StandardControlHeight#���  .+��   NarrowControlWidth#���  $��
5, 5, 5, 5q��� $��Maximize Speeds��  ThematicTextBlock#���    WideControlWidth#���  �  � 	Resources  �� MergedDictionaries �� Source   MaximizePrintSpeedsTextBlock �� 
TargetType �� BasedOn �� Property �� Value �� Triggers	 �� Binding
 �� ElementName �� Path �� Value��  �� $! /Views/GlobalResources.xaml?�%:  &	       7  �� " ��8   .���  � ��$  ����  � ��$ Right=� �� 	 �� $
 tgbtnMaximizeSpeed��$ 	IsChecked�	 False.a���  � ��$ False����  � ��$ Right=��� 	 �� $
 tgbtnMaximizeSpeed��$ 	IsChecked� True.a���  � ��$
 True����  � ��$
 Left=�.+�d� -   $��tgbtnMaximizeSpeed��  ThematicToggleButton#���   	 StandardToggleButtonWidth#��� 	  d� 	IsChecked �� $ MaximizeSpeeds� � ToolTip ��  
 ThematicToolTipTextBlock#��� 
 @>Set speeds to their maximum values as set in the Printer menu.�� #��[  J� IsHitTestVisible .  #���  �� $  
Horizontal=�$��Center=�#���  .+��� $%��/Resources/General/MaxSpeed.png��  ThematicControlImage#���  #���  �� $��MaxSpeed��#���  #���  ��   ThematicTextBox#���    StandardControlWidth#���  ���� $ MaxSpeed� �� Mode$ TwoWay=�.�� J� InputBindings �� 
 �� Key$ Enter�� �� Command �� *ManualSetAxisCommand+.�� �� #��� 
 HFThe actuator will accelerate to this speed for all following commands.�� DBPress Enter to or press the Execute button to execute the command..���� $
��mm/s��#���  #���  �� $  
Horizontal=�$��Center=�#���  .+��� $)��#/Resources/General/Acceleration.png��#���  #���  �� $��Acceleration��#���  #���  �� #���  #���  ���� $ Acceleration�$ TwoWay=�.�� �� $ Enter�� �� *ManualSetAxisCommand+.�� �� #��� 
 KIThe actuator will use this acceleration value for all following commands.�� DBPress Enter to or press the Execute button to execute the command..���� $��mm/s²��#���  #���  �� -   $��
btnExecute��$��Right=�$��Center=�  ThematicButtonBorder#���  $��10, 10, 10, 10q�._��� $
��Left=�$��Bottom=�  ThematicButton#���    StandardImageButtonHeight#���  ���� *ManualSetAxisCommand+���� .R��� $  
Horizontal=�
������ .+��� $$��/Resources/Buttons/Execute.png��#���  #���  $��
0, 0, 0, 0q��� $  Vertical=�$��Center=�.+��� $��Execute��#���  $��
0, 0, 0, 0q� �� #��� 
 MKUse this actuator for all following commands with the specified parameters.