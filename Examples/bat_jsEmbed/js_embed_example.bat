@if (@CodeSection == @Batch) @then
@echo off
setlocal

:: Execute this same file (%~f0) using the built-in JScript engine
cscript //nologo //E:jscript "%~f0" %*

:: Terminate the batch execution so it does not fall into JScript code
exit /b %errorlevel%
@end

// ============================================================================
// JScript / ECMAScript Area (Native Windows Script Host)
// ============================================================================

// --- 1. Class / Constructor Definition ---
function Dog(name, age) {
    // Instance properties (State)
    this.name = name;
    this.age = age;
}

// --- 2. Prototype Methods (Shared across all instances) ---
Dog.prototype.bark = function() {
    WScript.Echo(this.name + " says: Woof!");
};

Dog.prototype.getAge = function() {
    WScript.Echo(this.name + " is " + this.age + " years old.");
};

Dog.prototype.birthday = function() {
    this.age += 1;
    WScript.Echo("Happy birthday, " + this.name + "! Now " + this.age + ".");
};

// --- 3. Inheritance Example (Dog -> GuardDog) ---
function GuardDog(name, age, sector) {
    // Call super constructor
    Dog.call(this, name, age);
    this.sector = sector;
}

// Inherit prototype
GuardDog.prototype = new Dog();
GuardDog.prototype.constructor = GuardDog;

// Add / Override method
GuardDog.prototype.patrol = function() {
    WScript.Echo(this.name + " is patrolling Sector " + this.sector + ".");
};

// ============================================================================
// Execution / Instantiation
// ============================================================================

// Instantiating standard Dog objects
var dog1 = new Dog("Buddy", 3);
var dog2 = new Dog("Max", 5);

dog1.bark();
dog1.getAge();
dog1.birthday();

WScript.Echo("--------------------------------");

dog2.bark();
dog2.getAge();

WScript.Echo("--------------------------------");

// Instantiating derived GuardDog object
var guard = new GuardDog("Rex", 4, "A-North");
guard.bark();
guard.patrol();