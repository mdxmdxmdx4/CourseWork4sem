   M S B A M L   `   `   `  ����K  GAppForCourseWork, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:AppForCourseWorkAppForCourseWork  5   	   !    AppForCourseWork.MainWindow   5   	   W SPresentationCore, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:AppForCourseWork     5   	   -   5          � Title$  MDAir��5   	   $	��500��6   $	��850��6$   .��5	      �  � ColumnDefinitions 5
   
   �� 5      .+�5   
   �� $	��Top=�6    �� Orientation$ 
Horizontal=�6-   $��Right=�6F   $��0 5 5 0q�6b   .+�5      �� -   5         ��Name$ Acc_name��5      $�� ��6*   $��Center=�63   $��0 0 10 0q�6P   $��16(�6b   �� 5      $ Vertical=�6   .+�5      �� -   5        $[��UC:\Users\mdxbu\Labs\4SEMESTR\CourseWork\AppForCourseWork\AppForCourseWork\account.png��5      $��25��6w    � Cursor$
 Hand��5   4   �� 5      -   5         ��Name$ Open_Register_Window��5      
�������$��14(�5   (   $��0 10 0 0q�66   $
 Hand��6H   $��0q�6V   .��5      �� $��Регистрация��6    $��	Underline{�63   6Q   5      5      5      �� 5   
   -   5        $��Registration_Window��5   R   $	��400��6   $	��400��6   
����ح� �� CornerRadius$ 5��6A   $y�Hidden=�6m   ._�5      �� .+�5      �� -   5        $��Close_reg_window��5      $��X��5      
���  ��$
��Bold"�63   $��24(�6E   
��� � �$��Right=�6f   $
 Hand��6�   �� 5      $ 
Horizontal=�6   $��Center=�66   .+�5      �� $?��9Заполните поля для регистрации��6    $��18(�6F   $��SemiBold"�6T   5        5!      �� 5"      $��
20 20 20 0q�6   .+�5#      �� $��ФИО��6    $��16(�6+   $��DemiBold"�69   �� 5$      -   5        $��Full_name_tb��5$      $��0 10 0 0q�5%      $��16(�6+   6;   �� 5&      $#��Серия пасспорта��6    $��16(�67   $��DemiBold"�6E   $��0 20 0 0q�6[   6o   �� 5'      -   5        $��Passport_tb��5'      $��0 10 0 0q�5(      $��16(�6+   6;   �� 5)      $��Дата рождения��6    $��16(�65   $��DemiBold"�6C   $��0 20 0 0q�6Y   6m   (  "System.Windows.Controls.DatePicker  5*      -	   5        $��Bday_registration��5*   !   $��0 10 0 0q�5+      $��14(�6+   6;   �� 5,      -
   5        $��Register��5,      $
 Hand��6.   $��40��5.      $��0 50 0 0q�6%   $	��240��67   $��16(�6C   ��5/      �� 50       �� 
TargetType" ��6.   .R�51   "   � .+�52   &   �� .+�53   *   F� 
���� ����54   .   �� 55   2    �� Rect$ 
0,0,240,40�6D   	 �� RadiusX$	 10Q�6V   
 �� RadiusY$
 10Q�6c   57   +   F� 58   *   
���������59   .   �� 5:   2   $ 
4,4,232,32�6D   $	 8Q�6V   $
 8Q�6b   5<   +   5=   '   �� 5>   &   $��Center=�67   $��Center=�6T   6q   5?   #   5@      .��5B      �� $*��$Зарегистрироваться��6$   $��18(�6>   6N   5C      5D      5E      5F      �� 5H   
   -   5        $��Login_window��5H      $ 6��6%   $	��550��5I      $	��300��6   
����p��$y�Hidden=�6A   ._�5J      � .+�5K      �� $��Right=�6   .+�5L      �� -   5        $��Close_login_window��5L   Q   $��X��6    
����i��
���   �$��20(�5M   B   $
��Bold"�6P   $��0 5 5 0q�6b   $
 Hand��6s   6�   5N      �� 5O      $��Center=�6   $��0 20 0 0q�6:   .+�5P      �� -   5        $��	Greetings��5P   ]   $&�� Добро пожаловать!��6    $��20(�69   $��SemiBold"�6G   6]   5Q      �� 5R      $��
80 50 80 0q�6   .+�5S      �� $#��Серия пасспорта��6    $��16(�67   $��SemiBold"�6E   $��0 15 0 0q�6[   6o   �� 5U      -   5        $��Login_tb��5U   >   $��18(�6   $��0 10 0 0q�6,   6P   �� 5W      $��Пароль��6    $��16(�6.   $��SemiBold"�6<   $��0 20 0 0q�6R   6f   G� 5X      -   5        $��	Passw_box��5X   B   $��18(�6"   $��0 10 0 0q�60   6U   �� 5Y      -   5        $��	Enter_btn��5Z      $��0 35 0 0q�5Y      $��40��6/   $	��270��6;   
����  �$
 Hand��5Z   .   .��5[      �� $��
Войти��6$   
�������$��18(�6I   $��SemiBold"�6W   6o   5\      5]      5^      5_      �� 5a   
   -   5        $��Search_border��5a      $	��650��5b      $	��300��6   
�������$ 10��6?   $��2q�6Q   
���  �� �� Padding$ 30q�5c   '   $y�Visible=�64   ._�5d      �� .+�5e      �  5f      �� 5g      �� 5h       � RowDefinitions 5j      �� 5k      $��40 �6(   �� 5l      $
��50px �6(   �� 5m      $��100px �6(   .+�5o      �� $C��=Выберите параметры вашего полёта��6    $��24(�6H   $
��Bold"�6V   $	��Top=�5p       $��2��68   $��0��6L   $
��Left=�6Y   $	��590��6t   6�   �� 5q      $��1��6    $'��!Место отправления��6-   $��Bottom=�6G   $��20(�6b   $��Arial)�6p   6�   �� 5r      $��1��6    $��2��6-   $%��Место назначения��6=   $��	-25 0 0 0q�6V   $��Bottom=�6i   $��20(�6�   $��Arial)�6�   6�   �� 5s      $��Дата��6    $��1��6,   $��2��69   $��Right=�6I   $��20(�6e   $��Arial)�6s   $��Bottom=�6�   $��0 0 25 0q�6�   6�   �� 5t      $ 
Horizontal=�6!   $��100px��6:   $��LeftToRight=�6I   $��2��6e   $��2��6y   .+�5u      �� -   5         ��Name$ 	departure��5u   "   $��	0 10 50 0q�65   $��Center=�6H   $��200px��6c   $
��24px(�6q   $��Arial Rounded MT Bold)�5v   "    �� 	MaxLength$ 20��6E   6T   �� 5x      $��->��6$   $��Center=�6.   $��	-32 0 0 0q�6I   $��35(�6\   $��Black"�6j   �� 5y      -   5        $ destination��5y   "   $��Center=�68   $��	10 0 0 -9q�6S   $��200px��6f   $
��24px(�6t   $��Arial Rounded MT Bold)�5z   "   $ 20��6E   6T     5|      -   5          Name$ dPicker��5|   %   $
��30px��66   $��Center=�6D   $��
15 0 0 -15q�6_   
������    IsTodayHighlighted$
 True��5}   %    � ForceCursor$
 True��6?   $��Verdana)�6R   5~      5      �� 5�      -   5        $��Search��5�   '   $��150px��6   $
��35px��5�      $��Center=�6'   ���	#FF80BBA6$
��Bold"�6[   $
 Hand��6m   ��5�      �� 5�      " ��6*   .R�5�      � .+�5�   "   �� .+�5�   &   F� ���	#FF80BBA6��5�   *   �� 5�   .   $ 
0,0,150,35�6@   $	 10Q�6R   $
 10Q�6_   5�   '   5�   #   �� 5�   "   $��Center=�63   $��Center=�6P   6m   5�      5�      .��5�      �� $��
Поиск��6    $��18(�6-   6=   5�      5�      5�      5�      5�      