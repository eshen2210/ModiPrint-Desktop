   M S B A M L   `   `   `  ����D  @ModiPrint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null�kclr-namespace:ModiPrint.Views.StatusAndControlViews.EquipmentSelectionViews.EquipmentSelectionSettingsViews]ModiPrint.Views.StatusAndControlViews.EquipmentSelectionViews.EquipmentSelectionSettingsViews  5      k    eModiPrint.Views.StatusAndControlViews.EquipmentSelectionViews.EquipmentSelectionSettingsViews.SetAxis   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      }localkclr-namespace:ModiPrint.Views.StatusAndControlViews.EquipmentSelectionViews.EquipmentSelectionSettingsViews     5      .��5      � $��Center=�6   .+�5	   
   ��   �� Orientation$  Vertical=�6   .+�5      �� $  
Horizontal=�6   $��Center=�5      .+�5      �� $!��/Resources/General/Axis.png��6   $��Center=�5         TallControlHeight#���   5      $��
5, 5, 5, 5q�5      �� 5      $  
Horizontal=�6   .+�5      �� $��
Set Axis: ��6      ThematicHeaderTextBlock#���  5       $��
5, 5, 0, 5q�5       �� 5      #���  5       $��
0, 5, 5, 5q�5       ��5       �� *
AxisName+5       5      5      �� 5      $��
Parameters��6     ThematicGroupBox#���  5      .��5      �� $  Vertical=�6   $��Center=�5       .+�5#      �� $  
Horizontal=�6!     StandardControlHeight#���  5$   !   .+�5&      �   NarrowControlWidth#���  6   $��
5, 5, 5, 5q�5'      �� 5)      $��Maximize Speeds��6$     ThematicTextBlock#���  5*   $     WideControlWidth#���  5+   $   � 5-       � 	Resources 5.       �� MergedDictionaries �� Source   MaximizePrintSpeedsTextBlock �� 
TargetType �� BasedOn �� Property �� Value �� Triggers	 �� Binding
 �� ElementName �� Path �� Value�� 5/   "    50   &   �� 51   *   $! /Views/GlobalResources.xaml?�6=   %�  &	       7  �� 54   &   " ��6Q   8   .�55   *   ��  � ��$  ��6A   �� 56   *    � ��$ Right=�6P    57   *   �� 58   .   	 6:   �� $
 tgbtnMaximizeSpeed��$ 	IsChecked�	 False6}   .a�59   2   ��  � ��$ False��6I   �� 5:   2    � ��$ Right=�6X   5;   /   �� 5<   .   	 6:   �� $
 tgbtnMaximizeSpeed��$ 	IsChecked� True6}   .a�5=   2   ��  � ��$
 True��6I   �� 5>   2    � ��$
 Left=�6X   5?   /   5A   '   5B   #   .+�5E      d� -   5        $��tgbtnMaximizeSpeed��5E   +     ThematicToggleButton#���  5G   +    	 StandardToggleButtonWidth#��� 	 5H   +    d� 	IsChecked 5F   +   �� $ MaximizeSpeeds� � ToolTip 5I   "   �� 5J   &    
 ThematicToolTipTextBlock#��� 
 60   @>Set speeds to their maximum values as set in the Printer menu.5L   '   5N      �� 5O      #��[ 6(    J� IsHitTestVisible .  #���  5Q   (   5R      5S      �� 5V      $  
Horizontal=�6!   $��Center=�5W   !   #���  5X   !   .+�5Z      �� $%��/Resources/General/MaxSpeed.png��6      ThematicControlImage#���  5[       #���  5\       �� 5]      $��MaxSpeed��6$   #���  5^   $   #���  5_   $   �� 5`        ThematicTextBox#���  5a   "     StandardControlWidth#���  5b   "   ��5`   "   �� $ MaxSpeed� �� Mode$ TwoWay=�.��5b   Q    J� InputBindings 5c      �� 5d   "   
 �� Key$ Enter��6-    �� Command 69   �� *ManualSetAxisCommand+.��5e   5    5f      �� 5g   "   #��� 
 6,   HFThe actuator will accelerate to this speed for all following commands.�� 5i   &   DBPress Enter to or press the Execute button to execute the command.5k   #   .��5l   /   5m      �� 5n      $
��mm/s��6$   #���  5o   $   #���  5p   $   5q      �� 5t      $  
Horizontal=�6!   $��Center=�5u   !   #���  5v   !   .+�5x      �� $)��#/Resources/General/Acceleration.png��6    #���  5y       #���  5z       �� 5{      $��Acceleration��6$   #���  5|   $   #���  5}   $   �� 5~      #���  5   "   #���  5�   "   ��5~   "   �� $ Acceleration�$ TwoWay=�.��5�   Q    5�      �� 5�   "   $ Enter��6-    69   �� *ManualSetAxisCommand+.��5�   5    5�      �� 5�   "   #��� 
 6,   KIThe actuator will use this acceleration value for all following commands.�� 5�   &   DBPress Enter to or press the Execute button to execute the command.5�   #   .��5�   /   5�      �� 5�      $��mm/s²��6$   #���  5�   $   #���  5�   $   5�      5�      5�      �� 5�      -   5        $��
btnExecute��5�      $��Right=�5�      $��Center=�61     ThematicButtonBorder#���  5�      $��10, 10, 10, 10q�5�      ._�5�      �� $
��Left=�5�      $��Bottom=�64     ThematicButton#���  5�        StandardImageButtonHeight#���  5�      ��5�      �� *ManualSetAxisCommand+��5�      �� 5�      .R�5�      �� $  
Horizontal=�6)   
������ .+�5�   "   �� $$��/Resources/Buttons/Execute.png��6(   #���  5�   (   #���  5�   (   $��
0, 0, 0, 0q�5�   (   �� 5�   "   $  Vertical=�6-   $��Center=�5�   -   .+�5�   &   �� $��Execute��60   #���  5�   0   $��
0, 0, 0, 0q�5�   0   5�   #   5�      5�       5�      �� 5�      #��� 
 6$   MKUse this actuator for all following commands with the specified parameters.5�      5�      5�      5�      5�      5�      