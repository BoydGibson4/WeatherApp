Imports System
Imports System.Net.Http
Imports System.Threading.Tasks
Imports Newtonsoft.Json.Linq

' Define the main module of the application
Module Program

    ' API key for accessing OpenWeatherMap; you can get a free api key from: https://openweathermap.org/api
    Private Const API_KEY As String = "you need to get your own from openweather"

    ' The main entry point of the application
    Sub Main(args As String())
        ' Display the title of the application
        Console.WriteLine("Weather App")
        Console.WriteLine("-----------")

        ' Prompt the user to enter a city name
        Console.Write("Enter a city name: ")
        Dim cityName As String = Console.ReadLine() ' Read the user's input

        ' Asynchronously fetch weather data for the entered city
        Dim weatherData As Task = FetchWeatherDataAsync(cityName)
        weatherData.Wait() ' Wait for the asynchronous task to complete

        ' Wait for the user to press a key before closing the application
        Console.WriteLine("Press any key to exit...")
        Console.ReadKey()
    End Sub

    ' Asynchronous function to fetch weather data from the OpenWeatherMap API
    Private Async Function FetchWeatherDataAsync(city As String) As Task
        ' Create an instance of HttpClient to send HTTP requests
        Using client As New HttpClient()
            Try
                ' Construct the API request URL with the city name, API key, and units set to metric (Celsius)
                Dim url As String = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={API_KEY}&units=metric"
                
                ' Send a GET request to the API and await the response
                Dim response As HttpResponseMessage = Await client.GetAsync(url)

                ' Check if the response status code indicates success (status code 200)
                If response.IsSuccessStatusCode Then
                    ' Read the response content as a string
                    Dim responseData As String = Await response.Content.ReadAsStringAsync()

                    ' Parse the JSON response data into a JObject for easy access to fields
                    Dim weatherDetails As JObject = JObject.Parse(responseData)

                    ' Display the weather details to the user
                    Console.WriteLine($"City: {weatherDetails("name")}") ' The name of the city
                    Console.WriteLine($"Temperature: {weatherDetails("main")("temp")} °C") ' Current temperature in Celsius
                    Console.WriteLine($"Weather: {weatherDetails("weather")(0)("description")}") ' Weather description (e.g., clear sky)
                    Console.WriteLine($"Humidity: {weatherDetails("main")("humidity")} %") ' Humidity percentage
                    Console.WriteLine($"Wind Speed: {weatherDetails("wind")("speed")} m/s") ' Wind speed in meters per second
                Else
                    ' If the response status code is not successful, display an error message
                    Dim errorContent As String = Await response.Content.ReadAsStringAsync() ' Read the error content
                    Console.WriteLine($"Error: Unable to fetch weather data. Status Code: {response.StatusCode}")
                    Console.WriteLine($"Error Content: {errorContent}") ' Display the error details
                End If

            ' Catch and handle specific HTTP request exceptions
            Catch ex As HttpRequestException
                Console.WriteLine("HttpRequestException: " & ex.Message)

            ' Catch and handle any other general exceptions
            Catch ex As Exception
                Console.WriteLine("Exception: " & ex.Message)
            End Try
        End Using
    End Function

End Module
