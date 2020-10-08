Feature: Subscriptions

Scenario Outline: When search then only items with text presented
	Given User logged in
	When 'Subscriptions' tab opened
	And Button 'Deactivate' available
	And Input having placeholder 'Search' recieved typing 'HTML'
	And Button 'Deactivate' available
	And Only items with 'HTML' title presented

Scenario Outline: When search then close button revert search
	Given User logged in
	When 'Subscriptions' tab opened
	And Button 'Deactivate' available
	And Input having placeholder 'Search' recieved typing 'HTML'
	And Button 'Deactivate' available
	And Only items with 'HTML' title presented
	And 'x' button clicked
	And Button 'Deactivate' available
	And Item with 'Unit testing' title presented

Scenario Outline: When user has no topics then no topics shown
	When Input having placeholder 'Enter login' recieved typing 'tester4'
	And Input having placeholder 'Enter password' recieved typing '1234567890-'
	And 'Sign-in!' button clicked
	When 'Subscriptions' tab opened
	And Text 'Subscriptions not found' presented on page

Scenario Outline: Deactivate topic succeessed
	Given User logged in
	When 'Subscriptions' tab opened
	And Item with 'mynewtesttoping' title presented
	And This resource has 'Deactivate' button
	And This resource has no 'Activate' button
	And Button 'Deactivate' available
	And 'Deactivate' button clicked
	And Button 'Activate' available
	When This resource has no 'Deactivate' button
	And This resource has 'Activate' button
	Then Page refreshed
	When Item with 'mynewtesttoping' title presented
	When This resource has no 'Deactivate' button
	And This resource has 'Activate' button
	And 'Activate' button clicked

Scenario Outline: Activate topic succeessed
	When Input having placeholder 'Enter login' recieved typing 'tester3'
	And Input having placeholder 'Enter password' recieved typing '1234567890'
	And 'Sign-in!' button clicked
	When 'Subscriptions' tab opened
	And Items with 'Front' title not presented
	When 'Knowledge Base' tab opened
	And Button 'Approve' available
	And Items with 'Front' title not presented
	And Input having placeholder 'Search' recieved typing 'testing'
	And 'Search' button clicked
	And Button 'Subscribe' available
	And 'Subscribe' button clicked
	And Text 'Subscribe for a topic' presented on page
	And Input having placeholder 'Title' recieved typing 'Front'
	And Input having placeholder 'Tags' recieved typing ', html, front, js, css'
	When 'OK' button clicked
	And Text 'Are you sure' presented on page
	And 'OK' button clicked
	When Tab 'Knowledge Base' available
	And 'Subscriptions' tab opened
	And Item with 'Front' title presented
	And 'Activate' button clicked
	And Text 'Topic properties' presented on page
	Then Choose date 'Start date and time' in future
	And Choose interval '7:0'
	When Input having placeholder 'Days:Hours:Minutes' recieved typing '1'
	When Button 'OK' available
	And 'OK' button clicked
	And Button 'Deactivate' available
	Then Page refreshed
	When Item with 'Front' title presented
	When This resource has 'Deactivate' button
	And This resource has no 'Activate' button
	And 'Deactivate' button clicked

Scenario Outline: Activation topic is unavailable when invalid data in past
	Given User logged in
	When 'Subscriptions' tab opened
	And Item with 'Unit testing' title presented
	And This resource has 'Activate' button
	When This resource has no 'Deactivate' button
	And 'Activate' button clicked
	And Text 'Topic properties' presented on page
	Then Choose date 'Start date and time' in past
	When Button 'OK' disabled

Scenario Outline: Activation topic succeessed valid interval
	Given User logged in
	When 'Subscriptions' tab opened
	And Item with 'Unit testing' title presented
	And This resource has 'Activate' button
	When This resource has no 'Deactivate' button
	And 'Activate' button clicked
	And Text 'Topic properties' presented on page
	Then Choose date 'Start date and time' in future
	And Choose interval '1:0:0'
	When Button 'OK' available
	And Button 'OK' enabled

Scenario Outline: Activation topic is unavailable when incorrect interval typed
	Given User logged in
	When 'Subscriptions' tab opened
	And Item with 'Unit testing' title presented
	And This resource has 'Activate' button
	When This resource has no 'Deactivate' button
	And 'Activate' button clicked
	And Text 'Topic properties' presented on page
	Then Choose date 'Start date and time' in past
	When Input having placeholder 'Days:Hours:Minutes' recieved typing '1:1:1:1'
	And Button 'OK' disabled

Scenario Outline: Cancel when activating topic returns on topics page
	Given User logged in
	When 'Subscriptions' tab opened
	And Item with 'Unit testing' title presented
	And This resource has 'Activate' button
	When This resource has no 'Deactivate' button
	And 'Activate' button clicked
	And Text 'Topic properties' presented on page
	And Button 'Cancel' available
	And 'Cancel' button clicked
	And Item with 'Unit testing' title presented
	And This resource has 'Activate' button
	When This resource has no 'Deactivate' button


Scenario Outline: Delete topic succeessed
	When Input having placeholder 'Enter login' recieved typing 'tester2'
	And Input having placeholder 'Enter password' recieved typing '1234567890-'
	And 'Sign-in!' button clicked
	When 'Subscriptions' tab opened
	And Items with 'HTML Study' title not presented
	When 'Knowledge Base' tab opened
	And Button 'Approve' available
	And Items with 'HTML Study' title not presented
	And Input having placeholder 'Search' recieved typing 'testing'
	And 'Search' button clicked
	And Button 'Subscribe' available
	And 'Subscribe' button clicked
	And Text 'Subscribe for a topic' presented on page
	And Input having placeholder 'Title' recieved typing 'HTML Study'
	And Input having placeholder 'Tags' recieved typing 'html, css'
	When 'OK' button clicked
	And Text 'Are you sure' presented on page
	And 'OK' button clicked
	When Tab 'Knowledge Base' available
	And 'Subscriptions' tab opened
	And Item with 'HTML Study' title presented
	And 'Activate' button clicked
	And Text 'Topic properties' presented on page
	When Button 'Delete' available
	And 'Delete' button clicked
	And Tab 'Subscriptions' available
	And Text 'HTML Study' not presented on page

Scenario Outline: Content parts shown when click on topic
	Given User logged in
	When 'Subscriptions' tab opened
	And Item with 'HTML' title presented
	And Clicked on this item
	And Text 'htmlbook.ru | Для тех, кто делает сайты' presented on page


Scenario Outline: Full content of content parts available on chevron click
	Given User logged in
	When 'Subscriptions' tab opened
	And Item with 'HTML' title presented
	And Clicked on this item
	And Item with 'htmlbook.ru | Для тех, кто делает сайты' title presented
	And Shevron-down clicked on this item for full text

