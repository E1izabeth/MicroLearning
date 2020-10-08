Feature: Settings, Common

Scenario Outline: Functionality restricted when login not activated profile
	When Input having placeholder 'Enter login' recieved typing 'tester4'
	And Input having placeholder 'Enter password' recieved typing '1234567890-'
	And 'Sign-in!' button clicked
	Then Alert 'Please, activate your profile first!' appeared
	And Toast with text 'Forbidden' appeared
	When Tab 'Knowledge Base' available
	And Link 'Knowledge Base' clicked
	Then Alert 'Please, activate your profile first!' appeared
	And Toast with text 'Forbidden' appeared
	When Tab 'Settings' available
	And Link 'Settings' clicked
	And Button 'Request new activation link!' available
	Then Profile login should be equal to 'tester4'

Scenario Outline: Request activation link succeessed
	When Input having placeholder 'Enter login' recieved typing 'tester4'
	And Input having placeholder 'Enter password' recieved typing '1234567890-'
	And 'Sign-in!' button clicked
	Then Alert 'Please, activate your profile first!' appeared
	And Toast with text 'Forbidden' appeared
	When 'Settings' tab opened
	And Button 'Request new activation link!' available
	And Input having id 'data-activation-email' recieved typing 'tt@tt.tt'
	And 'Request new activation link!' button clicked
	Then Toast with text 'Email having activation link was sent to your email' appeared

Scenario Outline: Request activation toast is closable
	When Input having placeholder 'Enter login' recieved typing 'tester4'
	And Input having placeholder 'Enter password' recieved typing '1234567890-'
	And 'Sign-in!' button clicked
	Then Alert 'Please, activate your profile first!' appeared
	And Toast with text 'Forbidden' appeared
	When 'Settings' tab opened
	And Button 'Request new activation link!' available
	And Input having id 'data-activation-email' recieved typing 'tt@tt.tt'
	And 'Request new activation link!' button clicked
	Then Toast with text 'Email having activation link was sent to your email' appeared
	When Toast with text 'Email having activation link was sent to your email' closed
	Then Toast with text 'Email having activation link was sent to your email' disappeared

Scenario Outline: Password successfully changed
	When Input having placeholder 'Enter login' recieved typing 'panama'
	And Input having placeholder 'Enter password' recieved typing '1234567890'
	And 'Sign-in!' button clicked
	When 'Settings' tab opened
	And Button 'Change password!' available
	And Input having placeholder 'New password' recieved typing '1234567890'
	And Input having placeholder 'Repeat new password' recieved typing '1234567890'
	And Input having placeholder 'Current email' recieved typing 'k.k@l'
	And 'Change password!' button clicked
	Then Toast with text 'Your password was successfully changed' appeared

Scenario Outline: Password changing failed due to invalid old email
	Given User logged in
	When 'Settings' tab opened
	And Button 'Change password!' available
	And Input having placeholder 'New password' recieved typing '1234567890'
	And Input having placeholder 'Repeat new password' recieved typing '1234567890'
	And Input having placeholder 'Current email' recieved typing 'ttt@t.t'
	And 'Change password!' button clicked
	Then Toast with text 'Invalid old email' appeared

Scenario Outline: Validation when password changing failed due to
	Given User logged in
	When 'Settings' tab opened
	And Button 'Change password!' available
	And Input having placeholder 'New password' recieved typing '<password>'
	And Input having placeholder 'Repeat new password' recieved typing '<password2>'
	And Input having placeholder 'Current email' recieved typing '<email>'
	And 'Change password!' button clicked
	Then Input having placeholder '<placeholder>' is not valid

	Examples:
		| reason                 | password    | password2    | email    | placeholder   |
		| not the same passwords | 1234567890  | 1234567890-- | tt@tt.tt | New password  |
		| invalid email          | 1234567890- | 1234567890-  | ppppppp  | Current email |

Scenario Outline: Email successfully changed
	When Input having placeholder 'Enter login' recieved typing 'tester2'
	And Input having placeholder 'Enter password' recieved typing '1234567890-'
	And 'Sign-in!' button clicked
	When 'Settings' tab opened
	And Button 'Change email!' available
	And Input having id 'data-current-email' recieved typing 'tt@tt.tt'
	And Input having placeholder 'New email' recieved typing 'ttt@ttt.ttt'
	And Input having placeholder 'Repeat new email' recieved typing 'ttt@ttt.ttt'
	And Input having placeholder 'Current password' recieved typing '1234567890-'
	And 'Change email!' button clicked
	Then Toast with text 'Your email was successfully changed' appeared

Scenario Outline: Email changing failed due to
	Given User logged in
	When 'Settings' tab opened
	And Button 'Change email!' available
	And Input having id 'data-current-email' recieved typing '<current email>'
	And Input having placeholder 'New email' recieved typing '<new email>'
	And Input having placeholder 'Repeat new email' recieved typing '<new email2>'
	And Input having placeholder 'Current password' recieved typing '<password>'
	And 'Change email!' button clicked
	Then Toast with text 'Invalid old email or password' appeared

	Examples:
		| reason              | current email | new email | new email2 | password    |
		| wrong current email | wrong@t.t     | t@t.t     | t@t.t      | 1234567890- |
		| wrong password      | tt@tt.tt      | t@t.t     | t@t.t      | 0987654321  |

Scenario Outline: Validation when email changing failed due to
	Given User logged in
	When 'Settings' tab opened
	And Button 'Change email!' available
	And Input having id 'data-current-email' recieved typing '<current email>'
	And Input having placeholder 'New email' recieved typing '<new email>'
	And Input having placeholder 'Repeat new email' recieved typing '<new email2>'
	And Input having placeholder 'Current password' recieved typing '<password>'
	And 'Change email!' button clicked
	Then Input having id '<id>' is not valid

	Examples:
		| reason                | current email | new email   | new email2 | password    | id                     |
		| invalid current email | invalidmail   | tt@tt.tt    | tt@tt.tt   | 1234567890- | data-current-email     |
		| invalid new email     | tt@tt.tt      | invalidmail | t@t.t      | 1234567890- | data-mail-new-email    |
		| emails not matched    | tt@tt.tt      | ttt@ttt.ttt | t@t.t      | 1234567890- | data-mail-repeat-email |