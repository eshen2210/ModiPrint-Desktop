   M S B A M L   `   `   `  ���� D  @ModiPrint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null�Kclr-namespace:ModiPrint.Views.StatusAndControlViews.EquipmentSelectionViews=ModiPrint.Views.StatusAndControlViews.EquipmentSelectionViews  }Cclr-namespace:ModiPrint.ViewModels.PrinterViewModels.AxisViewModels5ModiPrint.ViewModels.PrinterViewModels.AxisViewModels  �Hclr-namespace:ModiPrint.ViewModels.PrinterViewModels.PrintheadViewModels:ModiPrint.ViewModels.PrinterViewModels.PrintheadViewModels  [2clr-namespace:ModiPrint.Utilities.ControlFunctions$ModiPrint.Utilities.ControlFunctions  Z    TModiPrint.Views.StatusAndControlViews.EquipmentSelectionViews.EquipmentSelectionView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  WlocalKclr-namespace:ModiPrint.Views.StatusAndControlViews.EquipmentSelectionViews  PvmAxisCclr-namespace:ModiPrint.ViewModels.PrinterViewModels.AxisViewModels  ZvmPrintheadHclr-namespace:ModiPrint.ViewModels.PrinterViewModels.PrintheadViewModels  JuControlFunctions2clr-namespace:ModiPrint.Utilities.ControlFunctions  .���   � 	Resources   �� MergedDictionaries �� Source   SetAxisButtonBorder �� 
TargetType  ThematicButtonBorder �� BasedOn �� Property �� Value �� TriggersF   @ModiPrint.ViewModels.PrinterViewModels.AxisViewModels.AxisStatus   BeingSet	 �� Value
 �� Binding   Active  SetPrintheadButtonBorderP   JModiPrint.ViewModels.PrinterViewModels.PrintheadViewModels.PrintheadStatus   Active��  �� $! /Views/GlobalResources.xaml?�%u  &	        7  &	 �     7  �� " ��8   .���  � ��$ 10q� �� #	 Z 
 �� *
AxisStatus+.a���  � ��
 �  ���� #	 Z 
 �� *
AxisStatus+.a���  � ��
 � ����� " ��8   .���  � ��$ 10q� �� #	 Z 
 �� *
AxisStatus+.a���  � ��
 �  ���� #	 Z 
 �� *PrintheadStatus+.a���  � ��
 � ���.+���  �� Orientation$ Vertical=�.+��� $ Vertical=�.+��� $��Set Actuator��  ThematicTextBlock#���  $��
0, 0, 0, 0q���  �� SelectedValuePath$
 Name��  ThematicImageComboBox#���  $��
0, 0, 0, 0q�  StandardImageComboBoxHeight#���    StandardImageComboBoxWidth#���  ���� *$"PrinterViewModel.AxisViewModelList+ �� SelectedValue �� *!ManualControlViewModel.AxisName+.  (System.Windows.Interactivity.Interaction   Triggers /  )System.Windows.Interactivity.EventTrigger     	EventName$ DropDownOpened��.  (System.Windows.Interactivity.TriggerBase   Actions. 6  0System.Windows.Interactivity.InvokeCommandAction     Command �� *1/ManualControlViewModel.AxisSelectionNullCommand+���� .R��� #��[   % !StandardImageComboBoxContentWidth#���  $��
0, 0, 0, 0q�._��� $ 
Horizontal=�$��Center=�$��Center=�.+��� $!��/Resources/General/Axis.png��  ThematicControlImage#���   	 StandardImageButtonHeight#��� 	 $��
0, 0, 0, 0q��� $ Vertical=�$��Center=�.+��� #���  $��
0, 0, 0, 0q����� *Name+ � ToolTip ��  
 ThematicToolTipTextBlock#��� 
 NLSet actuator parameters for the XYZ stage and/or set a different Z actuator.�� jhAll operations after setting actuator parameters will use these parameters until new parameters are set.�� geA yellow border around the actuator icon indicates that the printer is currently using that actuator.�� ��A red border around the actuator icon indicates that the printer has sent the command to set a new actuator to the microcontroller but the microcontroller has yet to execute those commands.�� $ Vertical=�.+��� $��Set Printhead��#���  $��
0, 0, 0, 0q��� $
 Name��#���  $��
0, 0, 0, 0q�#���  #���  ���� *)'PrinterViewModel.PrintheadViewModelList+ �� *&$ManualControlViewModel.PrintheadName+   $ DropDownOpened��.    �� *64ManualControlViewModel.PrintheadSelectionNullCommand+���� .R��� #��[ #���  $��
0, 0, 0, 0q�._��� $ 
Horizontal=�$��Center=�$��Center=�.+��� #���  #��� 	 $��
0, 0, 0, 0q����� *ImageSource+�� $ Vertical=�$��Center=�.+��� #���  $��
0, 0, 0, 0q����� *Name+ �� #��� 
 NLSet printhead parameters for the XYZ stage and/or set a different printhead.�� KIWhen switching printheads, the associated Z actuator will by set as well.�� kiAll operations after setting printhead parameters will use these parameters until new parameters are set.�� igA yellow border around the printhead icon indicates that the printer is currently using that printhead.�� ��A red border around the printhead icon indicates that the printer has sent the command to set a new printhead to the microcontroller but the microcontroller has yet to execute those commands.�� $ Vertical=� � DataContext �� *ManualControlViewModel+.+��� $��Print Style��#���  $��
0, 0, 0, 0q��� $ StyleID�� �� SelectedIndex$ 0��#���  $��
0, 0, 0, 0q�#���  #���  ���� *AxesPrintStylesList+ �� *AxesPrintStyle+���� .R��� #���  #���  $��
0, 0, 0, 0q�._��� $ 
Horizontal=�$��Center=�$��Center=�.+��� #���  #��� 	 $��
0, 0, 0, 0q����� *ImageSource+�� $ Vertical=�$��Center=�$��
5, 5, 5, 5q�.+��� #���  $��
0, 0, 0, 0q����� *DisplayString1+�� #���  $��
0, 0, 0, 0q����� *DisplayString2+ �� #��� 
 *(Set a different movement or print style.�� @>All commands executed in the Control menu will use this style.�� OMMovement will enable all movement commands to operate without the printheads.�� �Continuous will enable all movement commands to be accompanied by continual dispensing of the set printhead along the toolpath.�� ��Droplet will enable all movement commands to stop at regular intervals with the set printhead dispensing droplets at each interval.