let scenarioCount = 0;
let optionsCount = 2;
// Funksjon for å legge til et nytt scenario
function addScenario() {
    optionsCount = 2;
    scenarioCount++;
    const scenarioDiv = document.createElement('div');
    scenarioDiv.id = `scenario${scenarioCount}`;
    scenarioDiv.innerHTML = `
        <div class="scenarioBox">
            <h3 class="scenarioHead">Scenario ${scenarioCount}</h3>
            <input type="text" class="titleInput" id="scenarioTitle${scenarioCount}" name="scenarioTitle${scenarioCount}" placeholder="Tittel" required>
            <div class="optionsDiv" id="options${scenarioCount}">
                <input class="optionInput" placeholder="Option 1" type="text" name="option${scenarioCount}[]" required>
                <input class="optionInput" placeholder="Option ${optionsCount}" type="text" name="option${scenarioCount}[]" required>
            </div>
            <button type="button" class="addOptionBtn" onclick="addOption(${scenarioCount})">Add Option</button>
        </div> 
    `;
    document.getElementById('scenarios').appendChild(scenarioDiv);
}

// Funksjon for å legge til et nytt alternativ i et scenario
function addOption(scenarioId) {

    if (optionsCount < 3) {
        optionsCount ++;
        const optionsDiv = document.getElementById(`options${scenarioId}`);
        const input = document.createElement('input');
        input.type = 'text';
        input.name = `option${scenarioId}[]`;
        input.className = `optionInput`;
        input.placeholder = `Option ${optionsCount}`;
        input.required = true;
        optionsDiv.appendChild(input);
    } else {
        alert("Maks 3 alternativer")
    }
}

// Lytter til skjemaets submit-hendelse
document.getElementById('createBtn').addEventListener('click', function() {
    event.preventDefault();
    // Vis lastemelding
    document.getElementById('loading').classList.add('active');

    // Samler data og sender den til serveren
    const formData = new FormData(document.getElementById('createForm'));
    const data = {
        title: formData.get('title'),
        scenarios: []
    };

    // Samler scenarier og deres alternativer
    for (let i = 1; i <= scenarioCount; i++) {
        const scenarioTitle = formData.get(`scenarioTitle${i}`);
        const options = formData.getAll(`option${i}[]`);
        data.scenarios.push({ title: scenarioTitle, options: options });
    }

    // Sender dataen til serveren som en POST-forespørsel
    fetch('http://localhost:5142/api/Sessions', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
        
    })
    .then(response => {
        if (!response.ok) {
            console.log(data);
            throw new Error('Network response was not ok ' + response.statusText);
        }
        return response.json();
    })
    .then(result => {
        console.log('Show created:', result);
        alert('Show created successfully!');
        // Omadresserer eller oppdaterer siden for å vise den nye sesjonen
        window.location.reload();
    })
    .catch(error => {
        console.error('Error creating show:', error);
        alert('Failed to create show. Please try again.');
    })
    .finally(() => {
        // Skjul lastemelding
        document.getElementById('loading').classList.remove('active');
    });
});
