   M S B A M L   `   `   `  ����   BackTestWindow#    BackTestWindow.BacktestWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Icon$   .\Resources\Laboratory.ico�� � Title$ BacktestWindow��$	��400��$
��1000��   LightBackground#��[  .���  � RowDefinitions �� $
��Auto ��� $
��Auto ��� $��* ��� $
��Auto �.+�� $��0��.+�o�  o� 
IsMainMenu . .@�m� $��Load Strategy�� m� Command �� *LoadStrategyDLLCmd+m� $��	Load Data�� �� *LoadTickDataCmd+m� $��Restart Test�� �� *ResetStrategyCmd+�� m� $��Request Historical Data�� �� *ReqHistDataCmd+m� $��Convert Historical Data�� �� *ConvertHistDataCmd+m� $��Plot Historical Data�� �� *PlotHistDataCmd+�� � $��1��.+���  �� Orientation$ 
Horizontal=�.+��� $��	Duration:��$��60��  
Foreground#��[ �� -   $��1s��$��50���� -   $��1m��$��50���� -   $��5m��$��50���� -   $	��10m��$��50���� -   $	��30m��$��50���� -   $��1h��$��50���� -   $	��End��$��50��� $��2��.+��� .@��� $��Messages��.���� -   $��Listbox1������  �� Path$ MessageList��� $��Ticks��.��&   System.Windows.Controls.DataGrid  -	   $��dgTick��   CanUserAddRows .  $��Stretch=����� $ 	TickTable��� $��
Indicators��.��  -
   $��dgIndicator�� .  $��Stretch=����� $ IndicatorTable��� $��Position��.��  -   $��
dgPosition�� .  $��Stretch=����� $ PositionTable��� $��Orders��.��  -   $��dgOrder�� .  $��Stretch=����� $ 
OrderTable��� $��Fills��.��  -   $��dgFill�� .  $��Stretch=����� $ 	FillTable��� $��Results��.��  -   $��dgResult�� .  $��Stretch=����� $ ResultTable��� $��Optimize��.���  � ColumnDefinitions �� $
��401* ��� $
��381* � �� $
��Auto ��� $��* �.+�� $��0��$��2��.+��� $ 
Horizontal=�.+��� #��[  #��[ .��Optimize On Parameter�� -   	 ��Name$	 cbOptimVariable��#��[  #��[ ���� $ OptimVariables�
 �� SelectedItem
 �� $ SelectedVariable��� #��[  #��[ .��Starting at:�� -    ��Name$ 	tbStartAt��$��50������ $ StartAt��� #��[  #��[ .��Finishing at:�� -   $ 
tbFinishat��$��50������ $ FinishAt��� #��[  #��[ .��
Step size:�� -   $ 
tbStepSize��$��50������ $ StepSize��� #��[  #��[ .��	Ojective:�� -   $	 
cbOptimObj������ $ 	OptimObjs�
 �� $ SelectedObj��� $��minimize��#��[  #��[ �� $��Start��$��80������ *StartOptimizationCmd+� $��1��$��2�� �� $
��auto ��� $
��auto ��� $��* �.+��� $��0��$
��Left=����� $ OptimumResultsList� �� SelectedIndex �� *SelectedOptimumResult+�� $��Right=�$��Stretch=�$��1�� �� ResizeBehavior$ PreviousAndNext=�$��5�����	#FFBCBCBC  -   $��	dgOptimum��$��2�� .  $��Stretch=����� $ OptimumResultsTable�� $��3��.+�]� ��. .+��� #��[  #��[ $��Right=�$
��Auto������ $ TickFileStatus��� #��[  #��[ $��Right=�$
��Auto������ $ 	DLLStatus��� #��[  #��[ $
��Auto������ $ 
MainStatus�