   M S B A M L   `   `   `  ����  HX_IGerm    HX_IGerm.MainWindow   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35a ]DevExpress.Xpf.Core.v16.1, Version=16.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49aF 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   ;dx2http://schemas.devexpress.com/winfx/2008/xaml/core  5   	   <dxb2http://schemas.devexpress.com/winfx/2008/xaml/bars  5   	   c _DevExpress.Xpf.Ribbon.v16.1, Version=16.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a>dxr4http://schemas.devexpress.com/winfx/2008/xaml/ribbon  5   	   ?dxe5http://schemas.devexpress.com/winfx/2008/xaml/editors  5   	     � Title$  
MainWindow��5   	   $	��450��6   $	��725��6)   .��5	      �  � RowDefinitions 5
   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   )  #DevExpress.Xpf.Ribbon.RibbonControl  $��0��6      RibbonStyle$ 
Office2010=�6)      
Categories. 5      5  /DevExpress.Xpf.Ribbon.RibbonDefaultPageCategory  2  ,DevExpress.Xpf.Ribbon.RibbonPageCategoryBase   Pages. 5      &   DevExpress.Xpf.Ribbon.RibbonPage     Caption)  #DevExpress.Xpf.Core.ObjectConverter$
 Home 6!      Groups. 5      +  %DevExpress.Xpf.Ribbon.RibbonPageGroup     Caption$
 File��6*      Items. 5      '  !DevExpress.Xpf.Bars.BarButtonItem  -   5        	  Name$	 newItem��5   ,   !  DevExpress.Xpf.Bars.BarItem
   Content$	
 New 6=      Glyph 6K   *	  $DevExpress.Xpf.Core.DXImageExtension	   	  Image1
  +DevExpress.Xpf.Core.Native.DXImageConverter$ New_16x16.png
    
LargeGlyph 6t   	  $ New_32x32.png
 *  $DevExpress.Xpf.Bars.BarItemSeparator  5      *  $DevExpress.Xpf.Ribbon.BarButtonGroup  5      -   5          Name$ editorGroup��5   -      Items. 5      %  DevExpress.Xpf.Bars.BarEditItem  -   5          Name$ editItem��5   .      	EditWidth$ 80X�6f      EditSettings 6@   I  CDevExpress.Xpf.Editors.Settings.Extension.ComboBoxSettingsExtension  6f   5        5      -   5        $ alignmentGroup��5   -   . 5      &   DevExpress.Xpf.Bars.BarCheckItem  -   5          Name$ leftCheckItem��5   /   $
 
Align Left 6F      
GroupIndex$ 1��6�      	IsChecked$
 TrueY�6�    6[   	  $ AlignLeft_16x16.png
 6�     5      -   5        $ rightCheckItem��5   /   $
 Align Right 6G   $ 1��6�    6]   	  $ AlignRight_16x16.png
 6�     5      -   5        $ justifyCheckItem��5   /   $
 Align Justify 6I   $ 1��6�    6a   	  $ AlignJustify_16x16.png
 6�   5        5         5!      -   5        $	 
saveAsItem��5!   ,   $
 Save As 6@    6R   	  $ SaveAs_32x32.png
 5"      5#      5$      5%      2  ,DevExpress.Xpf.Ribbon.RibbonStatusBarControl  5&   
   $��2��6%      
RightItems 5'        5(      -	   5        $	 	printItem��5(   $   $
 Print 67    6G   	  $ Print_16x16.png
 5*      5+      5,      