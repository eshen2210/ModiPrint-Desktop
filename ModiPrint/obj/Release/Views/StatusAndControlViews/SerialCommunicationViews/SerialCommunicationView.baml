   M S B A M L   `   `   `  ���� c  _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity  D @ModiPrint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null�Lclr-namespace:ModiPrint.Views.StatusAndControlViews.SerialCommunicationViews>ModiPrint.Views.StatusAndControlViews.SerialCommunicationViews ;"clr-namespace:ModiPrint.ViewModelsModiPrint.ViewModels \   VModiPrint.Views.StatusAndControlViews.SerialCommunicationViews.SerialCommunicationView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ^localLclr-namespace:ModiPrint.Views.StatusAndControlViews.SerialCommunicationViews     1vm"clr-namespace:ModiPrint.ViewModels     .����   �� Orientation$  Vertical=�.+��  � ColumnDefinitions �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $��* � � 	Resources  �� MergedDictionaries �� Source   ConnectionButtonBorder �� 
TargetType  ThematicButtonBorder �� BasedOn �� Triggers �� Binding	 �� Value
 �� Property �� Value  DisconnectionButtonBorder��  �� $! /Views/GlobalResources.xaml?�%�   &	        7  &	 j     7  �� " ��8    ��  �� *
IsPortOpen+	 True.a��� 
 � ��
 � ����� 
 � ��$ 10q��� " ��8    ��  �� *
IsPortOpen+		 False.a��� 
 � ��
 � ����� 
 � ��$ 10q�.+��� $��0��$  Vertical=�.+��� $��Serial Port��  ThematicTextBlock#���  �� -   $��cboSerialPorts��  StandardControlHeight#���    WideControlWidth#���    ThematicComboBox#���  ���� *	PortNames+.   (System.Windows.Interactivity.Interaction   Triggers /   )System.Windows.Interactivity.EventTrigger     	EventName$ DropDownOpened��.   (System.Windows.Interactivity.TriggerBase   Actions. 6   0System.Windows.Interactivity.InvokeCommandAction     Command �� *RefreshPortNamesCommand+ � ToolTip ��   ThematicToolTipTextBlock#���  wuSelect the serial port that the microcontroller USB cable is connected to then press the Connect button to the right.�� ��To see what serial port the microcontroller is connected to, go to Device Manager (not a part of this program) then the PORT tab.�� usYou can also guess the serial port. If you guess correctly, the serial message display should tell you "Connected".�� $��1��$��Right=�#��[  ._��� $��Connect��  ThematicButton#���   	 StandardImageButtonHeight#��� 	 ���� *SerialConnectCommand+����  �� ElementName$ cboSerialPorts�� �� Path$
 Text����� .R��� $  
Horizontal=�
������ .+��� $$��/Resources/Buttons/Connect.png��#��� 	  
 ThematicControlImage#��� 
 $��
0, 0, 0, 0q��� $��Connect��$��Center=�#���   �� #���  {ySelect the serial port that the microcontroller USB cable is connected to (drop-down to the left) then press this button.�� trIf a connection is established (to the appropriate microcontroller or not), this button will have a yellow border.�� wuIf a proper connection to the microcontroller is established, the serial message display should tell you "Connected".�� ~|It is recommended that you do not connect to the microcontroller until 10 seconds after giving power to the microcontroller.� $��3�� �� $��* ��� $
��Auto �.+��� -   $��tbManualCommand��$��0��#���    ThematicTextBox#���  ���� *ManualSerialSendMessage+ �� Mode$ TwoWay=�.�� �� #���  pnType in a command to be directly sent to the microcontroller. Then press the Send Message button to the right.�� ��All acceptable commands that can be sent to the microcontroller can be found under the "ModiPrint GCode Documentation.docx" file on Github..���� $��1��$��Right=�#���  ._��� #���  #��� 	 ���� *ManualAppendMessageCommand+���� $ tbManualCommand��$
 Text����� .R��� $  
Horizontal=�
������ .+��� $%��/Resources/Buttons/SendFile.png��#��� 	 #��� 
 $��
0, 0, 0, 0q��� $  Vertical=�$��Center=�$��
5, 5, 5, 5q�.+��� $
��Send��$��Center=�#���  $��
0, 0, 0, 0q��� $��Message��$��Center=�#���  $��
0, 0, 0, 0q� �� #���  VTSend a command (entered in the textbox to the left) directly to the microcontroller.�� NLAs with all commands in menu, a serial connection must be established first.