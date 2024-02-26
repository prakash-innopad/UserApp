
document.addEventListener("DOMContentLoaded", () => {
    var countryDropdown = document.getElementById("CountryList");
    let url = 'http://localhost:5000/api/account/country';
    fetch(url, {
        method: 'GET'
    })
        .then(Result => Result.json())
        .then(data => {

            console.log(data);
            data.result.forEach(country => {
                let option = document.createElement('option');
                option.text = country.name;
                option.value = country.id;
                countryDropdown.appendChild(option);
            });
        })
        .catch(errorMsg => { console.log(errorMsg); });


    var stateDropdown = document.getElementById("StateList");
    countryDropdown.addEventListener("change", () => {
        stateDropdown.length = 1;
        let url = `http://localhost:5000/api/account/state/${countryDropdown.value}`;
        fetch(url, {
            method: 'GET'
        })
            .then(Result => Result.json())
            .then(data => {
                console.log(data);
                data.result.forEach(state => {
                    let option = document.createElement('option');
                    option.text = state.name;
                    option.value = state.id;
                    stateDropdown.appendChild(option);
                });
            })
    })

    var cityDropdown = document.getElementById("CityList");
    stateDropdown.addEventListener("change", () => {
        cityDropdown.length = 1;
        let url = `http://localhost:5000/api/account/city/${stateDropdown.value}`;
        fetch(url, {
            method: 'GET'
        })
            .then(Result => Result.json())
            .then(data => {
                console.log(data);
                data.result.forEach(city => {
                    let option = document.createElement('option');
                    option.text = city.name;
                    option.value = city.id;
                    cityDropdown.appendChild(option);
                });
            })
    })

    countryDropdown.addEventListener("change", () => {
        stateDropdown.length = 1;
        cityDropdown.length = 1;
    });
    cityDropdown.addEventListener("change", () => {
        document.getElementById("address").value = `${(cityDropdown.options[cityDropdown.selectedIndex]).textContent}, ${(stateDropdown.options[stateDropdown.selectedIndex]).textContent}, ${(countryDropdown.options[countryDropdown.selectedIndex]).textContent}`;
    });

})
