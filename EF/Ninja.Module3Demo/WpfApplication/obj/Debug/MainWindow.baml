   M S B A M L   `   `   `  ����I  EWpfApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:WpfApplicationWpfApplication  5   	   N JNinjaDomain.Classes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullV>clr-namespace:NinjaDomain.Classes;assembly=NinjaDomain.ClassesNinjaDomain.Classes 5   	       WpfApplication.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:WpfApplication     5   	   RClasses>clr-namespace:NinjaDomain.Classes;assembly=NinjaDomain.Classes     5   	   -   5          � Title$  
MainWindow��5	   	   $	��350��6   $	��525��6)    � 	Resources 5
         ninjaViewSource ! ninjaEquipmentOwnedViewSource �� Source �� Source��%_   &	        &	      7   S� 5   
   6   �� 5   
    6E   �� *EquipmentOwned+8   5      .��5      �  � DataContext# [ 6   .+�5   
   � -   5         �Name$ grid1��5      $	��Top=�6   $��
33,240,0,0q�66   $
��Left=�6J   # [  6e    � RowDefinitions 5      �� 5      $
��Auto �6     � ColumnDefinitions 5      �� 5      $
��Auto �6#   �� 5      $
��Auto �6#   .+�5      �� $��Center=�6   $��0��6/   $��3q�6<   $
��Left=�6G   $��0��6b   $��Date Of Birth:��6r   (  "System.Windows.Controls.DatePicker  5      -   5          Name$ dateOfBirthDatePicker��5      $��Center=�68   $��0��5   Y   $��3q�6f   $
��Left=�6q   $��1��6�   	   SelectedDate	 5   S   �� *DateOfBirth+
 �� Mode$
 TwoWay=� �� NotifyOnValidationError .  �� ValidatesOnExceptions . 5   �   5      � 5   
   -   5        $ grid2��5      # [  6   $
��Left=�6M   $��
53,129,0,0q�6h   $	��Top=�6|    5      �� 5      $
��Auto �6#   �� 5      $
��Auto �6#    5!      �� 5"      $
��Auto �6    .+�5$      �� $��Name:��6   $��0��6$   $
��Left=�64   $��3q�6O   $��0��6Z   $��Center=�6g   �� 5%      -   5         ��Name$ nameTextBox��5%      $��1��6+   $
��Left=�6;   $��23��6V   $��3q�6b   $��0��6m   $��Center=�5&   <   $	��120��6W   ��5%   z   �� *Name+$
 TwoWay=� .  . 5&   W   5'      � 5(   
   -   5        $ grid3��5(      # [  6   $
��Left=�6M   $��
83,203,0,0q�6h   $	��Top=�6|    5)      �� 5*      $
��Auto �6#   �� 5+      $
��Auto �6#    5-      �� 5.      $
��Auto �6    .+�50      �� $��Served In Oniwaban:��6   $��0��62   $
��Left=�6B   $��3q�6]   $��0��6h   $��Center=�6u   �� 51      -   5         ��Name$ servedInOniwabanCheckBox��51      $�� ��69   $��1��6D   $
��Left=�6T   $��3q�52   s   $��0��6~   $��Center=�6�    d� 	IsChecked 51   o   �� *ServedInOniwaban+$
 TwoWay=� .  . 52   �   53      � 54   
   -   5        $ grid4��54      # [  6   $
��Left=�6M   $��
62,166,0,0q�6h   $	��Top=�6|    55      �� 56      $
��Auto �6#   �� 57      $
��Auto �6#    59      �� 5:      $
��Auto �6    .+�5<      �� $��Clan:��6   $��0��6$   $
��Left=�64   $��3q�6O   $��0��6Z   $��Center=�6g   �� 5=      -	   5         ��Name$ clanComboBox��5=      $��1��6-    �� DisplayMemberPath$ ClanName��6=   $
��Left=�6Z   $
��Auto��6u    �� SelectedValuePath$ Id��5>      $��3q�6_   $��0��6j   $��Center=�6w   $	��120��6�   ��5=   �   ��  �� Path$
 Clan���5?      �� 5@      .R�5A      $� 5B      5D      5E      &   System.Windows.Controls.DataGrid  5F   
   -
   5          Name$ equipmentOwnedDataGrid��5F         AutoGenerateColumns .     EnableRowVirtualization . $��289,124,30.4,60.8q�6�      RowDetailsVisibilityMode$ VisibleWhenSelected=�6�   ��6n   ��    Columns 5G      0  *System.Windows.Controls.DataGridTextColumn  5H      -   5        idColumn��,  &System.Windows.Controls.DataGridColumn   Header$ Id��5H   N      Width5  /System.Windows.Controls.DataGridLengthConverter$ SizeToHeader 6Z      
Visibility$ Hidden=�6o   1  +System.Windows.Controls.DataGridBoundColumn   Binding 67   �� *Id+6o     5I      -   5        
nameColumn��$
 Name��5I   R   $ SizeToHeader 6`    69   �� *Name+6`     5J      -   5        
typeColumn��$
 Type��5J   R   $ SizeToHeader 6`    69   �� *Type+6`   5L      �� 5M   
   -   5         ��Name$ ninjaListBox��5M      $
 Name��6(   $ Id��6A   $
��Left=�6X   $	��107��6s   $��	23,17,0,0q�6�   $	��Top=�6�   $	��225��6�   6�   �� 5N   
   -   5         ��Name$ button��5N      $��	New Ninja��6!   $��Right=�65   $��0,69,190.4,0q�6Q   $	��Top=�6g   $��75��6   6�   �� 5O   
   -   5        $ button1��5O      $
��Save��6"   $��Right=�61   $��422,281,0,0q�6M   $	��Top=�6b   $��75��6z   6�   5P      5Q      