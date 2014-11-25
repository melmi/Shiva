# Shiva

Shiva is a simple class library for easier implementation of ViewModels. This humble class library is good for small projects.

The word *Shiva* in Persian means Eloquent.

## Why?

When I started using MVVM, I saw myself repeating a lot of code for each view model. I searched for existing libraries, but they were too complicated for me. So, I ended up with dynamic view models (after seeing [this article](http://www.codeproject.com/Articles/613610/Dynamic-View-Model)) and I tried to complete it.

## How to use?

Assume a model and a view like these:

Model:

```csharp
    class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }
```

View:

```xml
	<StackPanel Margin="5">
		<TextBlock Text="Given Name"/>
		<TextBox Text="{Binding FirstName}"/>
		<TextBlock Text="Family Name"/>
		<TextBox Text="{Binding LastName}"/>
		<TextBlock Text="Full Name"/>
		<TextBlock Text="{Binding FullName}"/>
		<TextBlock Text="Age"/>
		<TextBox Text="{Binding Age}"/>
	</StackPanel>
```

Writing a ViewModel for even such a simple Model is not so straightforward. So let's see what `Shiva` can do for us?

### Basic ViewModel

All of the magic of `Shiva` relies in `PersonDialogViewModel` class:

```csharp
    class PersonDialogViewModel : Shiva.ViewModelProxy<Models.Person>
    {
        public PersonDialogViewModel(Models.Person p)
        {
            Model = p;
        }
    }
```

Thats all. This ViewModel implements `INotifyPropertyChanged` and the View can be bound to it for good. `Shiva` automatically exposes all properties with following types:

- Enumerated types
- Simple types (`int`, `bool`, etc)
- `string`
- `DateTime`, `TimeSpan` and `DateTimeOffset`

### Validation

What if we want to add some validation to our ViewModel? `Shiva` utilizes a `Configuration` class in each `ViewModelProxy` that works fine for validation.

``` csharp
    class PersonDialogViewModel : Shiva.ViewModelProxy<Models.Person>
    {
        public PersonDialogViewModel(Models.Person p)
        {
            Model = p;

            Configuration.Property(() => Model.Age)
                         .Enforce(x => x > 0, "Age should be greater than zero");
        }
    }
```

### Dependent Properties

Consider a case that we want to have a `FullName` property in our ViewModel like this:

```csharp
        public string FullName { get { return Model.FirstName + " " + Model.LastName; } }
```

We can tell `Shiva` that whenever `FirstName` or `LastName` properties changed, it should fire `PropertyChanged` event for this event, too. Again in the constructor we will have:

```csharp
	   Configuration.Property(() => FullName)
	                .DependsOn(() => Model.FirstName)
	                .DependsOn(() => Model.LastName);
```

### What to do if user canceled changes?
`ViewModelProxy` also implements `IEditableObject` that can handle cancellation. The final ViewModel is here:

```csharp
    class PersonDialogViewModel : Shiva.ViewModelProxy<Models.Person>
    {
        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public string FullName { get { return Model.FirstName + " " + Model.LastName; } }

        public PersonDialogViewModel(Models.Person p)
        {
            Model = p;

            Configuration.Property(() => Model.Age)
                         .Enforce(x => x > 0, "Age should be greater than zero");

            Configuration.Property(() => FullName)
                         .DependsOn(() => Model.FirstName)
                         .DependsOn(() => Model.LastName);

            OkCommand = new RelayCommand(x => EndEdit());
            CancelCommand = new RelayCommand(x => CancelEdit());

            BeginEdit();
        }
    }
```

You may noticed that `Shiva` has a simple implementation of `RelayCommand`.

## License

Shiva is published under MIT license.