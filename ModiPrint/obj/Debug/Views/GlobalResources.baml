   M S B A M L   `   `   `  ����O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5      D @ModiPrint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:ModiPrint.ViewsModiPrint.Views 5      [2clr-namespace:ModiPrint.Utilities.IValueConverters$ModiPrint.Utilities.IValueConverters 5      S OToggleSwitch, Version=1.1.2.0, Culture=neutral, PublicKeyToken=8637099990568f75A0clr-namespace:ToggleSwitch;assembly=ToggleSwitchToggleSwitch 5      K*clr-namespace:ModiPrint.Views.ControlViewsModiPrint.Views.ControlViews 5      [2clr-namespace:ModiPrint.Utilities.ControlFunctions$ModiPrint.Utilities.ControlFunctions 5      �Pclr-namespace:ModiPrint.Models.PrinterModels.PrintheadModels.PrintheadTypeModelsBModiPrint.Models.PrinterModels.PrintheadModels.PrintheadTypeModels 5	      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   StandardControlHeight  TallControlHeight  StandardImageButtonHeight  StandardImageComboBoxHeight  StandardToggleButtonHeight  NarrowControlWidth  StandardControlWidth  WideControlWidth  ExtraWideControlWidth 	 StandardImageComboBoxWidth %
 !StandardImageComboBoxContentWidth  StandardToggleButtonWidth  SmallFontSize  StandardFontSize  HeaderControlFontSize  SuperscriptFontSize  HeaderTextFont  StandardTextFont  StandardBackground  StandardForeground  StandardBorder  PrimaryColor  SecondaryColor  TertiaryColor  ToolTipServiceShowDuration  ThematicBorder  �� 
TargetType �� Property �� Value  ThematicButton �� Triggers  ThematicButtonBorder P� Property P� Value  ThematicTextBlock  ThematicHeaderTextBlock  ThematicToolTipTextBlock  ThematicTextBox �� 
TargetType ��Name J� SnapsToDevicePixels	 ��Name
 �� Triggers �� 
TargetName1   +System.Windows.Input.InputBindingCollection   MovementOnEnter
 �� Key �� Command ! MotorPrintContinuousOnEnter " MotorPrintDropletOnEnter # ValvePrintContinuousOnEnter $ ValvePrintDropletOnEnter % XYZControlTextBox �� BasedOn �� Binding �� Value1  +ModiPrint.Utilities.ControlFunctions.Attach ^� 
Conditions �� Binding �� ValueV  PModiPrint.Models.PrinterModels.PrintheadModels.PrintheadTypeModels.PrintheadType   	Motorized   Valve & ThematicComboBoxItem ' ThematicComboBox d� 	IsChecked �� Path �� Mode �� RelativeSource � ColumnDefinitions �� CornerRadius F�Name J� IsHitTestVisible �� SelectionBoxItem �� SelectionBoxItemTemplate  ��Name! �� 
IsReadOnly" �� 
IsReadOnly# �� IsDropDownOpen$ � AllowsTransparency% � ActualWidth& �� MaxDropDownHeight' K� IsItemsHost ( ThematicImageComboBox ) ThematicControlImage * ThematicTabControlItem( �� 	Resources + Key , 
ItemBorder - IsSelectedItemBorder) ]� 
Conditions* �� Property+ �� 
SourceName . IsMouseOverItemBorder / ThematicTabControl 0 ThematicTabControlTextBlock 1 ThematicGroupBox 2 ThematicListView2  ,System.Windows.Controls.AlternationConverter,   Values 3 BackgroundConverterTealAqua- �� 	Converter 4 ThematicListBoxItem 5 ThematicToggleButton7  1ModiPrint.Views.ControlViews.OpaqueClickableImage 6 StandardBorderBrush. �� Color 7 StandardBackgroundBrush 8 HoverBorderBrush 9 SelectedBackgroundBrush : SelectedForegroundBrush ; DisabledForegroundBrush < NormalBrush = NormalBorderBrush > HorizontalNormalBrush ? HorizontalNormalBorderBrush @ 
GlyphBrush A PressedBrush/ �� 
StartPoint0 �� EndPoint1 � Color2 � Offset B ScrollBarLineButton C ScrollBarPageButton D ScrollBarThumb E VerticalScrollBar3 � RowDefinitions4 ]� DecreaseRepeatButton5 ]� Thumb6 ]� IncreaseRepeatButton F HorizontalScrollBar G ThematicScrollViewer7 �� VerticalOffset8 � Value9 �� ScrollableHeight: � Maximum; �� ViewportHeight< �� ViewportSize*= �� #ComputedVerticalScrollBarVisibility> �� Orientation? �� HorizontalOffset@ �� ScrollableWidthA �� ViewportWidth,B �� %ComputedHorizontalScrollBarVisibility�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      1System&clr-namespace:System;assembly=mscorlib  5      'localclr-namespace:ModiPrint.Views  5      9vc2clr-namespace:ModiPrint.Utilities.IValueConverters  5      AToggleSwitch0clr-namespace:ToggleSwitch;assembly=ToggleSwitch  5      7vControl*clr-namespace:ModiPrint.Views.ControlViews  5      9cf2clr-namespace:ModiPrint.Utilities.ControlFunctions  5      cmPrintheadTypePclr-namespace:ModiPrint.Models.PrinterModels.PrintheadModels.PrintheadTypeModels  5	      -   5        GlobalResourceDictionary��%�G  &	        &	      &	 4     &	 N     &	 h     &	 �     &	 �     &	 �     &	 �     &		 �     &	
     &	 #    &	 ?    &	 Y    &	 s    &	 �    &	 �    &	 �    &	 �    &	     &	 "    &	 ?    &	 [    &	 |    &	 �    &	 �    7  7  &	 �    7  7  7  7  &	     7  7  7  7  &	     7  7  7  7  &	 �    7  7  7  7  &	 �	    &	 
    7  7  7  7  7  7  7  7  &	     &	! f   &	" �   &	# 9   &	$ �   &	%     7  7   7 ! 7 " 7 # 7 $ &	& E    7  7  7  7  &	' _    7 & 7  7  7  7  7  7  7  7  7  7  7  7  &	(     7 & 7  7  7  7  7  7  7  7  7  7  7  7  &	) �"    7  &	* �#    7  7  7 , 7  7 , 7  7 , 7 - 7 . &	/ �'    7  7  7  7  7 * 7  &	0 �(    7  &	1 *)    7  7  &	2 �)    7  7  7 3 7  7  7  7 4 &	5 `,    7  7  7  &	6 �0    &	7 1    &	8 -1    &	9 K1    &	: i1    &	; �1    &	< �1    &	= �1    &	> �1    &	? 2    &	@ 2    &	A =2    &	B A3    7  7  7 @ 7 A 7 ; &	C �5    &	D _7    &	E �8    7 B 7 C 7 D 7  7  7 C 7 B &	F �;    7 B 7 C 7 D 7  7  7 C 7 B '�� �>    7 F 7 E &	G �@    X�5      35Q�66   X�5      80Q�62   X�5      60Q�6:   X�5      77Q�6<   X�5      35Q�6;   X�5      50Q�63   X�5      120Q�66   X�5      170Q�62   X�5      280Q�67   X�5      270Q�6<   X�5      229Q�6C   X�5      81.7Q�6<   X�5      12Q�6.   X�5       16Q�61   X�5!      22Q�66   X�5"      10Q�64   *�5#      	Serif)�6/   *�5$      
Sans Serif)�66   ��5'      #252525��6:   ��5(      	White��68   ��5)      	White��64   ��5*      Teal��61   ��5+      	Turquoise��68   ��5,      Gold��62   ��5/      
120000��6>   �� 54      "  ��6#   .�55   
   ��  � ��$ 
2, 2, 2, 2q�6,   �� 56   
    �@��Padding$ 5q�6$   �� 57   
    �@��CornerRadius$ 5��6)   �� 58   
    � ��8   6(   �� 59   
    �@^�ShowDuration8  68   �� 5:   
    � ��$ 
5, 5, 5, 5q�6#   5;      �� 5?      "  ��6#   .�5@   
   ��  � ��
 ���� 6(   �� 5A   
    � ��8   6'   �� 5B   
    � ��8  6'   �� 5C   
    � ��8  6%   �� 5D   
    � ��$ Center=�67   �� 5E   
    � ��$ Center=�65   �� 5F   
    �@^�ShowDuration8  68    5G   
   5J      �� 5L      "  ��6)   ThematicButtonBorder��.�5M   
   ��  � ��8   6'   �� 5N   
    � ��$ 
3, 3, 3, 3q�6,   �� 5O   
    �@��Padding$ 5q�6$   �� 5P   
    � ��$ Center=�60   �� 5Q   
    � ��$ Center=�6.   �� 5R   
    �@��CornerRadius$ 5��6)   �� 5S   
    � ��8  6(   �� 5T   
    �@^�ShowDuration8  68   �� 5U   
    � ��$ 
5, 5, 5, 5q�6#    5V   
   P� 5W       �@J�IsMouseOver . . �5X      ��  � ��8  6/   5Y      5[      �� 5_      "  ��6&   .�5`   
   ��  � ��8   6'   �� 5a   
    � ��8  6'   �� 5b   
    � ��8  6%   �� 5c   
    �@��TextAlignment$ Center=�6*   �� 5d   
    � ��$ Center=�6.   �� 5e   
    � ��$ Center=�60   �� 5f   
    �@^�ShowDuration8  68   �� 5g   
    � ��$ 
5, 5, 5, 5q�6#   5h      �� 5j      "  ��6,   .�5k   
   ��  � ��8   6'   �� 5l   
    � ��8  6'   �� 5m   
    � ��8  6%   �� 5n   
    �@��TextAlignment$ Center=�6*   �� 5o   
    � ��$ Center=�6.   �� 5p   
    � ��$ Center=�60   �� 5q   
    �@^�ShowDuration8  68   �� 5r   
    � ��$ 
5, 5, 5, 5q�6#   5s      �� 5u      "  ��6-   5v      �� 5z      "  ��6$   .�5{   
   ��  � ��8   6'   �� 5|   
    � ��8  6'   �� 5}   
    � ��8  6%   �� 5~   
    � ��8  6'   �� 5   
    � ��$ Center=�67   �� 5�   
    � ��$ Center=�65   �� 5�   
    �@��TextAlignment$ Center=�6*   �� 5�   
    � ��8  6(   �� 5�   
    � ��$ 2q�6,   �� 5�   
    �@^�ShowDuration8  68   �� 5�   
    � ��$ 
5, 5, 5, 5q�6#   �� 5�   
    � �� 5�      �� 5�      " ��6"   .R�5�      �� $ border��6   #��z��6-   #��z��6Y   #��z��6�    . ._�5�      �� $	 PART_ContentHost��6'   |�.  $��Hidden=�6S   $��Hidden=�6z   5�      
 5�      P� 5�       � {� .  . �5�      �� $ border��68    �@J�Opacity$
 0.56Q�6L   5�      P� 5�       �@J�IsMouseOver . . �5�      �� $ border��6<   8   � ��6P   5�      P� 5�       �@J�	IsFocused . . �5�      �� $ border��6<   8   � ��6P   5�      5�      5�      5�         5�      �� 5�   
   $ Enter��6    6!   �� *ManualMovementCommand+5�         5�      �� 5�   
   $ Enter��6    6!   �� *%#ManualMotorPrintWithMovementCommand+5�         5�      �� 5�   
   $ Enter��6    6!   �� * ManualMotorDropletPrintCommand+5�         5�      �� 5�   
   $ Enter��6    6!   �� *%#ManualValvePrintWithMovementCommand+5�         5�      �� 5�   
   $ Enter��6    6!   �� * ManualValveDropletPrintCommand+5�      �� 5�      "  ��6&   8    5�   
   �� 5�       6   �� *AxesPrintStyle+ Movement Only6=   .a�5�      ��  �@ InputBindings8  6<   5�      ^� 5�       5�      �� 5�       6    �� *AxesPrintStyle+ Continuous Print6C   �� 5�      # Z 6H    6    �� *ActivePrintheadType+6H   .0�5�      ��  �@ InputBindings8  6<   5�      ^� 5�       5�      �� 5�       6    �� *AxesPrintStyle+ Droplet Print6C   �� 5�      # Z 6H    6    �� *ActivePrintheadType+6H   .0�5�      ��  �@ InputBindings8  6<   5�      ^� 5�       5�      �� 5�       6    �� *AxesPrintStyle+ Continuous Print6C   �� 5�      # Z 6H    6    �� *ActivePrintheadType+6H   .0�5�      ��  �@ InputBindings8  6<   5�      ^� 5�       5�      �� 5�       6    �� *AxesPrintStyle+ Droplet Print6C   �� 5�      # Z 6H    6    �� *ActivePrintheadType+6H   .0�5�      ��  �@ InputBindings8  6<   5�      5�      �� 5�      "  ��6)   .�5�   
   ��  � ��8   6'   �� 5�   
    � ��
 ���� 6(   �� 5�   
    � ��8  6'   �� 5�   
    � ��8  6'   �� 5�   
    � ��8  6%   �� 5�   
    � ��$ Center=�67   �� 5�   
    � ��$ Center=�65   5�      �� 5�      "  ��6%   .�5�   
   ��  � ��8   6/   �� 5�   
    � ��$ 0q�6,   �� 5�   
    � ��8  6'   �� 5�   
    � ��8  6'   �� 5�   
    � ��8  6%   �� 5�   
    �@^�ShowDuration8  68   �� 5�   
    � ��$ 
5, 5, 5, 5q�6#   �� 5�   
    �@��
IsReadOnly . 6'   �� 5�   
    � �� 5�      �� 5�      " ��6"   .R�5�      � .+�5�      d� $��2��6'   |�.   6I   �� $ IsDropDownOpen�$ TwoWay=� �� *TemplatedParent+��5�      �� 5�   "   .R�5�   &   �  5�   *   �� 5�   .   $��5* �6?   �� 5�   .   $��* �6?   .+�5�   *   �� $ Border��61   $��2��6B   $ 5��6V   8�� 8�� $��3q�6�   �� 5�   *   $��0��61   $ 5,0,0,5��6A   $��1q�6Y   8�� 8�� $��3q�6�   F� 5�   *   $ Arrow��6/   $��1��6>   8��	 $��Center=�6v   $��Center=�6�   "����        � 	=  	= � z     	6�   5�   '   
 5�   &   P� 5�   *    �@J�IsMouseOver . . �5�   .   �� $ Border��65   8 
  � ��6_   5�   +   P� 5�   *    �@d�	IsChecked$
 trueY�6T   . �5   .   �� $ Border��65   8   � ��6_   5  +   5  #   5     �� 5     $��ContentSite��6+   $��Center=�6>   $��Center=�6[    .  #��z 6�   #��z 6�   #��z��6�   $��0, 0, 20, 0q�6;  �� 5     $  PART_EditableTextBox��6"   $y�Hidden=�6@   #" z! 6T   � 5     $��Popup��6    $��Bottom=�6-   #��z# 6@   $ . |�.  $��Slide=�6�   .��5	     � $��DropDown��6$    . #��z% 6O   #��z& 6x   .+�5
  "   �� $ DropDownBorder��6)   8�� 6A   �� 5  "    . .��5  &   �� ' . 61   5  #   5     5     5     5     5      5  
   5     �� 5     "  ��6*   .�5  
   ��  � ��8   6/   �� 5  
    � ��$ 0q�6,   �� 5  
    � ��8  6'   �� 5  
    � ��8  6'   �� 5  
    � ��8  6%   �� 5  
    �@^�ShowDuration8  68   �� 5  
    � ��$ 
5, 5, 5, 5q�6#   �� 5   
    �@��
IsReadOnly . 6'   �� 5!  
    � �� 5"     �� 5#     " ��6"   .R�5$     � .+�5%     d� $��2��6'   |�.   6I   �� $ IsDropDownOpen�$ TwoWay=� �� *TemplatedParent+��5&     �� 5'  "   .R�5(  &   �  5)  *   �� 5*  .   $��5* �6?   �� 5+  .   $��* �6?   .+�5-  *   �� $ Border��61   $��2��6B   $ 5��6V   8�� 8�� $��1q�6�   �� 5.  *   $��0��61   $ 5,0,0,5��6A   $��1q�6Y   8�� 8�� $��
1, 1, 1, 1q�6�   F� 5/  *   $ Arrow��6/   $��1��6>   8��	 $��Center=�6v   $��Center=�6�   "����        � 	=  	= � z     	6�   50  '   
 51  &   P� 52  *    �@J�IsMouseOver . . �53  .   �� $ Border��65   8 
  � ��6_   54  +   P� 55  *    �@d�	IsChecked$
 trueY�6T   . �56  .   �� $ Border��65   8   � ��6_   57  +   59  #   5;     �� 5<     $��ContentSite��6+   $��Center=�6>   $��Center=�6[    .  #��z 6�   #��z 6�   #��z��6�   $��0, 0, 40, 0q�6;  �� 5=     $  PART_EditableTextBox��6"   $y�Hidden=�6@   #" z! 6T   � 5>     $��Popup��6    $��Bottom=�6-   #��z# 6@   $ . |�.  $��Slide=�6�   .��5?     � $��DropDown��6$    . #��z% 6O   #��z& 6x   .+�5@  "   �� $ DropDownBorder��6)   8�� 6A   �� 5A  "    . .��5B  &   �� ' . 61   5C  #   5D     5E     5F     5G     5I      5J  
   5L     �� 5P     "  ��6)   .�5Q  
   ��  � ��$ Center=�60   �� 5R  
    � ��$ Center=�6.   �� 5S  
    � ��$ Uniform=�6$   �� 5T  
    �@^�ShowDuration8   68   �� 5U  
    � ��$ 
5, 5, 5, 5q�6#   5V     �� 5Z     "  ��6+   ( 5[  
   ���� 5\     
ItemBorder+ "  ��6'   8   .�5]     ��  � ��8  6/   �� 5^      � ��$ 
5, 5, 5, 5q�64   �� 5_      � ��$ 
0, 0, 0, 0q�6+   5`     �� 5b     IsSelectedItemBorder+ "  ��61   8  .�5c     ��  � ��8  60   5d     �� 5f     IsMouseOverItemBorder+ "  ��62   8  .�5g     ��  � ��8  6/   5h     5i     .�5k  
   ��  � �� 5l     �� 5m     " ��6"   .R�5n     �� $��Border��6   8�� ._�5p     �� $��Header��6+   $��Center=�5q  +   $��Center=�5r  +   5s     
 5t     P� 5u      �@��
IsSelected . . �5v     �� $ Border��6%   8   � ��6J   5w     ]� 5x     ) 5y     �� 5z  "   * �@��
IsSelected .  6B   �� 5{  "   $+ Border��6P   * �@J�IsMouseOver . /�5}     �� 5~  "   $ Border��6)   8   � ��6N   5�     5�     5�     5�     �� 5�     "  ��6'   .�5�  
   ��  � ��8   6'   �� 5�  
    � ��8  6'   �� 5�  
    � ��8  6%   �� 5�  
    � ��8  6'   �� 5�  
    � ��$ 0q�6,   �� 5�  
    � ��8  6/   �� 5�  
    �@^�ShowDuration8  68   �� 5�  
    � ��$ 
5, 5, 5, 5q�6#   5�     �� 5�     "  ��60   8   .�5�  
   ��  � ��$ 
0, 0, 0, 0q�6#   5�     �� 5�     "  ��6%   .�5�  
   ��  � ��8   6'   �� 5�  
    � ��8  6%   �� 5�  
    � ��$ 10, 10, 10, 10q�6#   5�     �� 5�     "  ��6%   ( 5�  
   ��  5�     BackgroundConverterTealAqua+ ., 5�     ��DarkSlateGray��61   ��5�     DarkTurquoise��61   5�     �� 5�     ThematicListBoxItem+ "  ��60   .�5�     ��  � ��8   6/   �� 5�      � ��8  6-   �� 5�      � �� 6/   ��  �� *Self+$% (ItemsControl.AlternationIndex)�8-  5�     5�     .�5�  
   ��  � ��# �  6'   �� 5�  
    � ��8  6'   �� 5�  
    � ��8  6'   �� 5�  
    � ��8  6%   �� 5�  
    �@��AlternationCount$ 2��6-   �� 5�  
    � ��8  6/   �� 5�  
    � ��$ 
5, 5, 5, 5q�6#   5�     �� 5�     "  d�6)   .�5�  
   ��  � ��8   6#   �� 5�  
    � ��8  6"   �� 5�  
    �@^�ShowDuration8  68   �� 5�  
    � ��$ 
5, 0, 5, 0q�6#   �� 5�  
    � �� 5�     �� 5�     .R�5�       $'��!/Resources/ToggleButtons/Left.png��64   5�     5�      5�  
   ]� 5�     ) 5�     �� 5�     * �@d�	IsChecked$ FalseY�65   �� 5�     * �@J�IsMouseOver . 67   /�5�     �� 5�      � �� 5�     �� 5�     .R�5�  "     $0��*/Resources/ToggleButtons/LeftMouseOver.png��6@   5�     5�     5�     ]� 5�     ) 5�     �� 5�     * �@d�	IsChecked$
 TrueY�66   �� 5�     * �@J�IsMouseOver .  67   /�5�     �� 5�      � �� 5�     �� 5�     .R�5�  "     $(��"/Resources/ToggleButtons/Right.png��6@   5�     5�     5�     ]� 5�     ) 5�     �� 5�     * �@d�	IsChecked$
 TrueY�65   �� 5�     * �@J�IsMouseOver . 67   /�5�     �� 5�      � �� 5�     �� 5�     .R�5�  "     $1��+/Resources/ToggleButtons/RightMouseOver.png��6@   5�     5�     5�     5�     �� 5      $
. #888��62   �� 5     $. Black��66   �� 5     $
. #DDD��6/   �� 5     $
. Gray��66   �� 5     $. White��66   �� 5     $
. #888��66   �� 5     $
. #888��6*   �� 5     $
. #888��60   �� 5     $. Black��64   �� 5	     $
. #888��6:   �� 5
     $
. #444��6)   �� 5     $	/ 0,0&�6/   $	0 0,1&�6@   ��5     � � 5     $
1 #BBB��6   $	2 0.0Q�6,   � 5     $
1 #EEE��6   $	2 0.1Q�6,   � 5     $
1 #EEE��6   $	2 0.9Q�6,   � 5     $
1 #FFF��6   $	2 1.0Q�6,   5     5     �� 5     "  ��6(   .�5  
   ��  � y�$ Hidden=�6'   �� 5  
    �@J�SnapsToDevicePixels . 60   �� 5  
    �@�OverridesDefaultStyle . 62   �� 5  
    � |� .  6&   �� 5  
    � �� 5     �� 5     " ��6"   .R�5     �� $��Border��6   $��1q�6+   $ 2��66   8��  8�� $��1q�5"     ._�5#     F� $��Center=�6   $��Center=�6<   8�� ��5%     �� $ Content� �� *TemplatedParent+5&     
 5'     P� 5(      � �� . . �5)     �� $ Border��6%   8   � ��6O   5*     P� 5+      � {� .  . �5,     �� 8   � ��6;   5-     5/     51     52     �� 54     "  ��6(   .�55  
   ��  � y�$ Hidden=�6'   �� 56  
    �@J�SnapsToDevicePixels . 60   �� 57  
    �@�OverridesDefaultStyle . 62   �� 58  
    � �� .  6&   �� 59  
    � |� .  6&   �� 5:  
    � �� 5;     �� 5<     " ��6"   .R�5=     �� 
���   �6   5>     5@     5A     �� 5C     "  n�6#   .�5D  
   ��  �@J�SnapsToDevicePixels . 60   �� 5E  
    �@�OverridesDefaultStyle . 62   �� 5F  
    � �� .  6&   �� 5G  
    � |� .  6&   �� 5H  
    � �� 5I     �� 5J     " n�6"   .R�5K     �� $ 2��6   #��z��5L     #��z��5M     $��0q�5N     $��8��5O     $��8,0,-2,0q�5P     5Q     5S     5T     �� 5V     " ��60   .R�5W  
   � 3 5X     �� 5Y     $��0��6    �� 5Z     $��0.00001* �6    �� 5[     $��0��6    .+�5]     �� $��3��6   $ 2��5^     
������ 5_     �� 5`     $��0��6   8��  $��18��5b     $��ScrollBar.LineUpCommand��6'   $��M 0 4 L 8 4 L 4 0 Z��5c     ]� 5d     $��
PART_Track��6   $��1��5e     ��. 4 5g     �� 5h     8�� $��ScrollBar.PageUpCommand��5i  #   5 5k     n� 5l     8�� $��1,0,1,0q�5m     8�� 8�� 5o     6 5q     �� 5r     8�� $��ScrollBar.PageDownCommand��5s  #   5u     �� 5v     $��3��6   8�� $��18��5x     $��ScrollBar.LineDownCommand��5y     $��M 0 0 L 4 4 L 8 0 Z��5z     5{     5|     �� 5~     " ��62   .R�5  
   �  5�     �� 5�     $��18��6#   �� 5�     $��0.00001* �6#   �� 5�     $��18��6#   .+�5�     �� $��3��6   $ 2��5�     ���#F0F0F05�     �� 5�     $��0��6   8��  $��18��5�     $��ScrollBar.LineLeftCommand��5�     $��M 4 0 L 4 8 L 0 4 Z��5�     ]� 5�     $��
PART_Track��6   $��1��5�     ��.  4 5�     �� 5�     8�� $��ScrollBar.PageLeftCommand��5�     5 5�     n� 5�     8�� $��0,1,0,1q�5�     8�� 8�� 5�     6 5�     �� 5�     8�� $ ��ScrollBar.PageRightCommand��5�  #   5�     �� 5�     $��3��6   8�� $��18��5�     $ ��ScrollBar.LineRightCommand��5�     $��M 0 0 L 4 4 L 0 8 Z��5�     5�     5�     �� 5�     "  ��6'   .�5�  
   ��  �@J�SnapsToDevicePixels . 60   �� 5�  
    �@�OverridesDefaultStyle . 62    5�  
   P� 5�      �@��Orientation$ 
Horizontal=�6-   . �5�     ��  � ��$
 Auto��6*   �� 5�      � ��$ 18��6+   �� 5�      � ��8   6-   5�     P� 5�      �@��Orientation$ Vertical=�6-   . �5�     ��  � ��$ 18��6*   �� 5�      � ��$
 Auto��6+   �� 5�      � ��8  6-   5�     5�     �� 5�     "  ��6)   .�5�  
   ��  �@�OverridesDefaultStyle . 62   �� 5�  
    � �� 5�     �� 5�     " ��6"   .R�5�     �  5�     �� 5�     $
��Auto �6/   �� 5�     3 5�     �� 5�     �� 5�     $
��Auto �6,   .+�5�     �� $��1��61   �� 5�     $��PART_VerticalScrollBar��6$   $��1��5�  $   $��Right=�64   #8 z7 5�  $   #: z9 5�  $   #< z; 5�  $   #y�z= 5�  $   �� 5�     $��PART_HorizontalScrollBar��6$   $> 
Horizontal=�5�  $   $��1��5�  $   $��1��61   #8 z? 5�  $   #: z@ 5�  $   #< zA 5�  $   #y�zB 5�  $   5�     5�     5�     5�     5�     