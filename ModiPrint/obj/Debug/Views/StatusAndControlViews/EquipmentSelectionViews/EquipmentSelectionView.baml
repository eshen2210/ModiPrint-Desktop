   M S B A M L   `   `   `  ����D  @ModiPrint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null�Kclr-namespace:ModiPrint.Views.StatusAndControlViews.EquipmentSelectionViews=ModiPrint.Views.StatusAndControlViews.EquipmentSelectionViews  5      }Cclr-namespace:ModiPrint.ViewModels.PrinterViewModels.AxisViewModels5ModiPrint.ViewModels.PrinterViewModels.AxisViewModels  5      �Hclr-namespace:ModiPrint.ViewModels.PrinterViewModels.PrintheadViewModels:ModiPrint.ViewModels.PrinterViewModels.PrintheadViewModels  5	      [2clr-namespace:ModiPrint.Utilities.ControlFunctions$ModiPrint.Utilities.ControlFunctions  5
      Z    TModiPrint.Views.StatusAndControlViews.EquipmentSelectionViews.EquipmentSelectionView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5      WlocalKclr-namespace:ModiPrint.Views.StatusAndControlViews.EquipmentSelectionViews  5      PvmAxisCclr-namespace:ModiPrint.ViewModels.PrinterViewModels.AxisViewModels  5      ZvmPrintheadHclr-namespace:ModiPrint.ViewModels.PrinterViewModels.PrintheadViewModels  5	      JuControlFunctions2clr-namespace:ModiPrint.Utilities.ControlFunctions  5
      .��5      �   � 	Resources  5   
    �� MergedDictionaries �� Source   SetAxisButtonBorder �� 
TargetType  ThematicButtonBorder �� BasedOn �� Property �� Value �� TriggersF   @ModiPrint.ViewModels.PrinterViewModels.AxisViewModels.AxisStatus   BeingSet	 �� Value
 �� Binding   Active  SetPrintheadButtonBorderP   JModiPrint.ViewModels.PrinterViewModels.PrintheadViewModels.PrintheadStatus   Active�� 5       5      �� 5      $! /Views/GlobalResources.xaml?�6)   %y  &	        7  &	 ,    7  �� 5      " ��64   8   .�5      ��  � ��$ 10q�68    5      �� 5      #	 Z 6E   
 6&   �� *
AxisStatus+.a�5      ��  � ��
 �  ��6<   5      �� 5      #	 Z 6E   
 6&   �� *
AxisStatus+.a�5      ��  � ��
 � ���6<   5      5      �� 5       " ��69   8   .�5!      ��  � ��$ 10q�68    5"      �� 5#      #	 Z 6E   
 6&   �� *
AxisStatus+.a�5$      ��  � ��
 �  ��6<   5%      �� 5&      #	 Z 6J   
 6&   �� *PrintheadStatus+.a�5'      ��  � ��
 � ���6<   5(      5*      5-      .+�50   
   ��  �� Orientation$ Vertical=�6   .+�53      �� $ Vertical=�6   .+�54      �� $��Set Actuator��6     ThematicTextBlock#���  55      $��
0, 0, 0, 0q�56      �� 57       �� SelectedValuePath$
 Name��58        ThematicImageComboBox#���  5:      $��
0, 0, 0, 0q�5;        StandardImageComboBoxHeight#���  5<        StandardImageComboBoxWidth#���  6R   ��57      �� *$"PrinterViewModel.AxisViewModelList+ �� SelectedValue 59      �� *!ManualControlViewModel.AxisName+.  (System.Windows.Interactivity.Interaction   Triggers 5=      /  )System.Windows.Interactivity.EventTrigger  5>         	EventName$ DropDownOpened��6)   .  (System.Windows.Interactivity.TriggerBase   Actions. 5?      6  0System.Windows.Interactivity.InvokeCommandAction     Command 64   �� *1/ManualControlViewModel.AxisSelectionNullCommand+5@      ��5B      �� 5C      .R�5D      �� #��[  6%    % !StandardImageComboBoxContentWidth#���  5E   %   $��
0, 0, 0, 0q�5F   %   ._�5G   "   �� $ 
Horizontal=�6-   $��Center=�5H   -   $��Center=�5I   -   .+�5J   &   �� $!��/Resources/General/Axis.png��6,     ThematicControlImage#���  5K   ,    	 StandardImageButtonHeight#��� 	 5L   ,   $��
0, 0, 0, 0q�5M   ,   �� 5N   &   $ Vertical=�61   $��Center=�5O   1   .+�5P   *   �� #���  5Q   4   $��
0, 0, 0, 0q�5R   4   ��5P   4   �� *Name+5R   4   5S   '   5T   #   5U      5V       � ToolTip 5X      �� 5Y       
 ThematicToolTipTextBlock#��� 
 6$   NLSet actuator parameters for the XYZ stage and/or set a different Z actuator.�� 5[      jhAll operations after setting actuator parameters will use these parameters until new parameters are set.�� 5]      geA yellow border around the actuator icon indicates that the printer is currently using that actuator.�� 5_      ��A red border around the actuator icon indicates that the printer has sent the command to set a new actuator to the microcontroller but the microcontroller has yet to execute those commands.5a      5c      5d      �� 5h      $ Vertical=�6   .+�5i      �� $��Set Printhead��6   #���  5j      $��
0, 0, 0, 0q�5k      �� 5m      $
 Name��5n      #���  5p      $��
0, 0, 0, 0q�5q      #���  5r      #���  6R   ��5m      �� *)'PrinterViewModel.PrintheadViewModelList+ 5o      �� *&$ManualControlViewModel.PrintheadName+ 5s        5t      $ DropDownOpened��6)   . 5u         64   �� *64ManualControlViewModel.PrintheadSelectionNullCommand+5v      ��5x      �� 5y      .R�5z      �� #��[ 6%   #���  5{   %   $��
0, 0, 0, 0q�5|   %   ._�5}   "   �� $ 
Horizontal=�6-   $��Center=�5~   -   $��Center=�5   -   .+�5�   &   �� #���  5�   ,   #��� 	 5�   ,   $��
0, 0, 0, 0q�5�   ,   ��5�   ,   �� *ImageSource+5�   ,   �� 5�   &   $ Vertical=�61   $��Center=�5�   1   .+�5�   *   �� #���  5�   4   $��
0, 0, 0, 0q�5�   4   ��5�   4   �� *Name+5�   4   5�   '   5�   #   5�      5�       5�      �� 5�      #��� 
 6$   NLSet printhead parameters for the XYZ stage and/or set a different printhead.�� 5�      KIWhen switching printheads, the associated Z actuator will by set as well.�� 5�      kiAll operations after setting printhead parameters will use these parameters until new parameters are set.�� 5�      igA yellow border around the printhead icon indicates that the printer is currently using that printhead.�� 5�      ��A red border around the printhead icon indicates that the printer has sent the command to set a new printhead to the microcontroller but the microcontroller has yet to execute those commands.5�      5�      5�      �� 5�      $ Vertical=�6    � DataContext 5�      �� *ManualControlViewModel+.+�5�      �� $��Print Style��6   #���  5�      $��
0, 0, 0, 0q�5�      �� 5�      $ StyleID��5�       �� SelectedIndex$ 0��5�      #���  5�      $��
0, 0, 0, 0q�5�      #���  5�      #���  6R   ��5�      �� *AxesPrintStylesList+ 5�      �� *AxesPrintStyle+��5�      �� 5�      .R�5�      �� #���  6%   #���  5�   %   $��
0, 0, 0, 0q�5�   %   ._�5�   "   �� $ 
Horizontal=�6-   $��Center=�5�   -   $��Center=�5�   -   .+�5�   &   �� #���  5�   ,   #��� 	 5�   ,   $��
0, 0, 0, 0q�5�   ,   ��5�   ,   �� *ImageSource+5�   ,   �� 5�   &   $ Vertical=�61   $��Center=�5�   1   $��
5, 5, 5, 5q�5�   1   .+�5�   *   �� #���  5�   4   $��
0, 0, 0, 0q�5�   4   ��5�   4   �� *DisplayString1+5�   4   �� 5�   *   #���  5�   4   $��
0, 0, 0, 0q�5�   4   ��5�   4   �� *DisplayString2+5�   4   5�   '   5�   #   5�      5�       5�      �� 5�      #��� 
 6$   *(Set a different movement or print style.�� 5�      @>All commands executed in the Control menu will use this style.�� 5�      OMMovement will enable all movement commands to operate without the printheads.�� 5�      �Continuous will enable all movement commands to be accompanied by continual dispensing of the set printhead along the toolpath.�� 5�      ��Droplet will enable all movement commands to stop at regular intervals with the set printhead dispensing droplets at each interval.5�      5�      5�      5�      5�      5�      