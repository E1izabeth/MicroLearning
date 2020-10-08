Feature: Home Page

Scenario Outline: Login existing user succesfull
	When Input having placeholder 'Enter login' recieved typing 'panama2'
	And Input having placeholder 'Enter password' recieved typing '1234567890-'
	And 'Sign-in!' button clicked
	And Tab 'Settings' available
	And Link 'Settings' clicked
	And Button 'Change email!' available
	Then Profile login should be equal to 'panama2'

Scenario Outline: Login failed with alert due to reason of
	When Input having placeholder 'Enter login' recieved typing '<login>'
	And Input having placeholder 'Enter password' recieved typing '<password>'
	And 'Sign-in!' button clicked
	Then Alert 'Invalid credentials' appeared

	Examples:
		| reason           | login   | password    |
		| invalid password | panama2 | 1234567890  |
		| invalid login    | panama  | 1234567890- |

Scenario Outline: Login validation failed due to reason of
	When Input having placeholder 'Enter login' recieved typing '<login>'
	And Input having placeholder 'Enter password' recieved typing '<password>'
	And 'Sign-in!' button clicked
	Then Input having placeholder '<invalidButtonPlaceholder>' is not valid

	Examples:
		| reason         | login   | password    | invalidButtonPlaceholder |
		| empty login    |         | 1234567890- | Enter login              |
		| empty password | panama2 |             | Enter password           |

Scenario Outline: Register successed and toast shown
	When 'Register' button clicked
	And Button 'Sign-up!' available
	And Register form filled with login 'Somelogin', email 'Some@mail.ru', password 'Somepassword' and password repeat 'Somepassword'
	And 'Sign-up!' button clicked
	Then Toast with text 'Email having activation link was sent to your email' appeared

Scenario Outline: Register failed with alert due to reason of
	When 'Register' button clicked
	And Button 'Sign-up!' available
	And Register form filled with login '<login>', email '<email>', password '<password>' and password repeat '<password2>'
	And 'Sign-up!' button clicked
	Then Alert '<alertText>' appeared

	Examples:
		| reason                 | login   | email          | password            | password2             | alertText                   |
		| existing login         | panama2 | valid@email.ru | somecorrectpassword | somecorrectpassword   | User panama2 already exists |
		| not matching passwords | newUser | valid@email.ru | somecorrectpassword | someincorrectpassword | Passwords are not matched   |

Scenario Outline: Register validation failed due to reason of
	When 'Register' button clicked
	And Button 'Sign-up!' available
	And Register form filled with login '<login>', email '<email>', password '<password>' and password repeat '<password2>'
	And 'Sign-up!' button clicked
	Then Input having placeholder '<invalidButtonPlaceholder>' is not valid

	Examples:
		| reason         | login   | email          | password            | password2           | invalidButtonPlaceholder |
		| empty login    |         | valid@email.ru | somecorrectpassword | somecorrectpassword | Enter login              |
		| invalid email  | newUser | invalidemail   | somecorrectpassword | somecorrectpassword | Enter email              |
		| empty email    | newUser |                | somecorrectpassword | somecorrectpassword | Enter email              |
		| empty password | newUser | valid@email.ru |                     | somecorrectpassword | Enter password           |

Scenario Outline: Recover successed with toast shown
	When 'Recover' button clicked
	And Button 'Recover!' available
	And Recover form filled with login 'tester3', email 't@t.t' and email repeat 't@t.t'
	And 'Recover!' button clicked
	Then Toast with text 'Email having restore link was sent to your email' appeared

Scenario Outline: Recover validation failed due to reason of
	When 'Recover' button clicked
	And Button 'Recover!' available
	And Recover form filled with login '<login>', email '<email>' and email repeat '<email2>'
	And 'Recover!' button clicked
	Then Input having placeholder '<invalidButtonPlaceholder>' is not valid

	Examples:
		| reason        | login   | email | email2 | invalidButtonPlaceholder |
		| empty login   |         | t@t.t | t@t.t  | Enter login              |
		| invalid email | tester3 | t@t.t | tttttt | Repeat email             |

Scenario Outline: Recover failed with alert due to reason of
	When 'Recover' button clicked
	And Button 'Recover!' available
	And Recover form filled with login '<login>', email '<email>' and email repeat '<email2>'
	And 'Recover!' button clicked
	Then Alert '<alertText>' appeared

	Examples:
		| reason          | login    | email    | email2   | alertText                         |
		| emails mismatch | tester3  | t@t.t    | tt@tt.tt | Emails are not matched            |
		| incorrect email | tester3  | tt@tt.tt | tt@tt.tt | User not found or incorrect email |
		| missing user    | unexists | t@t.t    | t@t.t    | User not found or incorrect email |

Scenario Outline: Register form reset after alert clears its state
	When 'Register' button clicked
	And Register form filled with login 'Somelogin', email 'Some@mail.ru', password 'Somepassword' and password repeat 'Somewrongpassword'
	And 'Sign-up!' button clicked
	Then Alert 'Passwords are not matched' appeared
	When 'Reset' button clicked
	Then Alert disappeared
	When Input having placeholder 'Enter login' contains empty value
	And Input having placeholder 'Enter email' contains empty value
	And Input having placeholder 'Enter password' contains empty value
	And Input having placeholder 'Repeat password' contains empty value

Scenario Outline: Recover form reset after alert clears its state
	When 'Recover' button clicked
	And Recover form filled with login 'Somelogin', email 'Some@mail.ru' and email repeat 'Some@gmail.ru'
	And 'Recover!' button clicked
	Then Alert 'Emails are not matched' appeared
	When 'Reset' button clicked
	Then Alert disappeared
	When Input having placeholder 'Enter login' contains empty value
	And Input having placeholder 'Enter email' contains empty value
	And Input having placeholder 'Repeat email' contains empty value

Scenario Outline: Home page Back button resets from
	When '<mode>' button clicked
	And '<mode>' mode opened
	And 'Back' button clicked
	Then 'Login' mode opened

	Examples:
		| mode     |
		| Recover  |
		| Register |

Scenario Outline: Register success toast is closable
	When 'Register' button clicked
	And Button 'Sign-up!' available
	And Register form filled with login 'tester33', email 't@t.t', password 'qwertyuiop[' and password repeat 'qwertyuiop['
	And 'Sign-up!' button clicked
	Then Toast with text '<toast text>' appeared
	When Toast with text '<toast text>' closed
	Then Toast with text '<toast text>' disappeared

	Examples:
		| case | toast text                                          |
		|      | Email having activation link was sent to your email |

Scenario Outline: Recover success toast is closable
	When 'Recover' button clicked
	And Button 'Recover' available
	And Recover form filled with login 'tester3', email 't@t.t' and email repeat 't@t.t'
	And 'Recover' button clicked
	Then Toast with text '<toast text>' appeared
	When Toast with text '<toast text>' closed
	Then Toast with text '<toast text>' disappeared

	Examples:
		| case | toast text                                       |
		|      | Email having restore link was sent to your email |

Scenario Outline: About page presented
	When Text 'About microlearning' not presented on page
	And Link 'About' clicked
	And Text 'About microlearning' presented on page
	And Link 'Home' clicked
	And Text 'About microlearning' not presented on page

Scenario Outline: Logout successfully happens
	Given User logged in
	When Button 'Logout' available
	And 'Logout' button clicked
	And Button 'Sign-in!' available