Feature: Knowledge Base

Scenario Outline: Test approve resource successed
	Given User logged in
	When 'Knowledge Base' tab opened
	And Item with 'Unit testing' title presented
	And This resource has 'Awaiting' badge
	And This resource has no 'Approved' badge
	And Button 'Approve' available
	And 'Approve' button clicked
	When This resource has no 'Approve' button
	And This resource has no 'Awaiting' badge
	And This resource has 'Approved' badge
	Then Page refreshed
	When Item with 'Unit testing' title presented
	And This resource has no 'Approve' button
	And This resource has no 'Awaiting' badge
	And This resource has 'Approved' badge

Scenario Outline: Add resource successed
	Given User logged in
	When 'Knowledge Base' tab opened
	And Button '+' available
	And '+' button clicked
	Then Create resource modal appeared
	When Input having placeholder 'Title' recieved typing 'SpecFlow'
	And Input having placeholder 'Url' recieved typing 'https://specflow.org/documentation/Calling-Steps-from-Step-Definitions/'
	And Input having placeholder 'Tags' recieved typing 'testing, specflow'
	And 'OK' button clicked
	And Text 'Are you sure' presented on page
	And 'OK' button clicked
	And Item with 'SpecFlow' title presented
	And This resource has 'Awaiting' badge

Scenario Outline: Cancel when adding resource successed
	Given User logged in
	When 'Knowledge Base' tab opened
	And Button '+' available
	And '+' button clicked
	Then Create resource modal appeared
	When Input having placeholder 'Title' recieved typing 'Driver Pattern'
	And Input having placeholder 'Url' recieved typing 'http://leitner.io/2015/11/14/driver-pattern-empowers-your-specflow-step-definitions/'
	And Input having placeholder 'Tags' recieved typing 'testing, specflow, driver'
	And 'Cancel' button clicked
	And Items with 'Driver Pattern' title not presented
	
Scenario Outline: Adding resource failed due to
	Given User logged in
	When 'Knowledge Base' tab opened
	And Button '+' available
	And '+' button clicked
	Then Create resource modal appeared
	When Input having placeholder 'Title' recieved typing '<title>'
	And Input having placeholder 'Url' recieved typing '<url>'
	And Input having placeholder 'Tags' recieved typing '<tags>'
	And 'OK' button clicked
	And Text 'Are you sure' presented on page
	And 'OK' button clicked
	Then Toast with text '<toast text>' appeared

	Examples:
	| Reason      | title          | tags              | toast text                                     | url                                                                                  |
	| empty title |                | testing, specflow | Resource title should not be empty             | http://leitner.io/2015/11/14/driver-pattern-empowers-your-specflow-step-definitions/ |
	| empty tags  | Driver Pattern |                   | Bad Request                                    | http://leitner.io/2015/11/14/driver-pattern-empowers-your-specflow-step-definitions/ |
	| empty url   | Driver Pattern | testing, specflow | Invalid URI: The hostname could not be parsed. |                                                                                      |


Scenario Outline: Suggest succeessed
	Given User logged in
	When 'Knowledge Base' tab opened
	And Button '+' available
	And '+' button clicked
	Then Create resource modal appeared
	When Input having placeholder 'Url' recieved typing 'https://habr.com/en/post/210288//'
	When 'Suggest tags and title' button clicked
	When Button 'Suggest tags and title' available
	And Input having placeholder 'Tags' contains value 'паттерны,  шаблоны,  паттерны проектирования,  шаблоны проектирования,  gof,  gang of four,  банда четырёх,  проектирование,  архитектура,  шпаргалка по паттернам'
	And Input having placeholder 'Title' contains value 'Шпаргалка по шаблонам проектирования / Habr'

Scenario Outline: Toast shown when suggested invalid url
	Given User logged in
	When 'Knowledge Base' tab opened
	And Button '+' available
	And '+' button clicked
	Then Create resource modal appeared
	When Input having placeholder 'Url' recieved typing 'invalidurl'
	And 'Suggest tags and title' button clicked
	Then Toast with text 'The remote name could not be resolved: 'invalidurl'' appeared

Scenario Outline: Search succeeded
	Given User logged in
	When 'Knowledge Base' tab opened
	And Button 'Approve' available
	And Input having placeholder 'Search' recieved typing 'testing'
	And 'Search' button clicked
	When Button 'Approve' available
	And Only items with 'testing' tags presented
	And Button 'x' available

Scenario Outline: Closing search succeeded
	Given User logged in
	When 'Knowledge Base' tab opened
	And Button 'Approve' available
	And Input having placeholder 'Search' recieved typing 'testing'
	And 'Search' button clicked
	When Button 'Approve' available
	And Only items with 'testing' title presented
	And Button 'x' available
	And 'x' button clicked
	And Tab 'Knowledge Base' available
	And Item with 'HTML' title presented

Scenario Outline: Search gave zero results when no such items
	Given User logged in
	When 'Knowledge Base' tab opened
	And Button 'Approve' available
	And Input having placeholder 'Search' recieved typing 'cat'
	And 'Search' button clicked
	And Button 'x' available
	And Text 'Nothing found' presented on page

Scenario Outline: Search by keywords failed due to invalid word
	Given User logged in
	When 'Knowledge Base' tab opened
	And Button 'Approve' available
	And Input having placeholder 'Search' recieved typing 'noresults'
	And 'Search' button clicked
	Then Toast with text 'No model to normalize word 'noresults' of nor language' appeared

Scenario Outline: Subscribe failed due to no title
	Given User logged in
	When 'Knowledge Base' tab opened
	And Button 'Approve' available
	And Input having placeholder 'Search' recieved typing 'testing'
	And 'Search' button clicked
	And Button 'Subscribe' available
	And 'Subscribe' button clicked
	And Text 'Subscribe for a topic' presented on page
	When 'OK' button clicked
	And Text 'Are you sure' presented on page
	And 'OK' button clicked
	Then Toast with text 'Topic title should not be empty' appeared

Scenario Outline: Subscribe successed
	Given User logged in
	When 'Knowledge Base' tab opened
	And Button 'Approve' available
	And Items with 'Front' title not presented
	And Input having placeholder 'Search' recieved typing 'testing'
	And 'Search' button clicked
	And Button 'Subscribe' available
	And 'Subscribe' button clicked
	And Text 'Subscribe for a topic' presented on page
	And Input having placeholder 'Title' recieved typing 'Front'
	And Input having placeholder 'Tags' recieved typing 'html, front, js, css'
	When 'OK' button clicked
	And Text 'Are you sure' presented on page
	And 'OK' button clicked
	When Tab 'Knowledge Base' available
	And 'Subscriptions' tab opened
	And Item with 'Front' title presented
