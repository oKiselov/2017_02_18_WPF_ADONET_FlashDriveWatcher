   M S B A M L   `   `   `  ����H  DWpfFlashDrive, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:WpfFlashDriveWpfFlashDrive  5   	       WpfFlashDrive.WindowScan   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:WpfFlashDrive     5   	     � Title$  
WindowScan��5   	   $	��410��6   $	��120��6(    � 
ResizeMode$ NoResize=�65    � WindowStyle$ SingleBorderWindow=�6K   
�������.��5	   
   �  � ColumnDefinitions 5
   
   �� 5      �� 5      �� 5      �� 5       � RowDefinitions 5   
   �� 5      �� 5      .+�5      �� -   5        $��ButtonStartScanning��5       � ToolTip$ Start scanning��60   $��0��6I   $��2��6Y   
������ $��2q�6�   6�   � 5   
   -   5        $��ProgressBarScan��5      $��0��6.   $��1��6;   $��3��6K   
������ $��3q�5      �� 5      -   5        $��	LabelScan��5      $��1��6%   $��1��65   $��2��6B   $��Status:��6V   $��Center=�6h   $��Verdana)�5      $��12(�6)   �� 5   
   -   5        $��1��5      $��3��6   $��Cancel��6.   $��Verdana)�6?   $��12(�6T   $��Expanded&�6b   $��DemiBold"�6y   
���OO/�$��Center=�5      $��Center=�66   
������ $��0,0,3,3q�6s   6�   5      5      