document.addEventListener("DOMContentLoaded", function() {
    console.log("DOM fully loaded and parsed");

    const form = document.getElementById("reviewForm");
    console.log("Form element:", form);

    if (!form) {
        console.error('Form element not found');
        return;
    }

    form.addEventListener("submit", async function (event) {
        event.preventDefault();

        // Extract sessionId and userId from the URL
        const urlParams = new URLSearchParams(window.location.search);
        const sessionId = urlParams.get('sessionId');
        const userId = urlParams.get('userId');

        if (!sessionId || !userId) {
            alert('Session ID or User ID is missing in the URL.');
            return;
        }

        const textElement = document.getElementById('text');
        console.log("Text element:", textElement);

        if (!textElement) {
            console.error('Text element not found');
            return;
        }

        const text = textElement.value;

        const review = {
            sessionId: parseInt(sessionId),
            userId: userId,
            text: text
        };

        try {
            const response = await fetch(`/api/reviews/${sessionId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(review)
            });

            if (response.ok) {
                const result = await response.json();
                alert('Review submitted successfully!');
            } else {
                const error = await response.json();
                alert(`Error: ${error.message}`);
            }
        } catch (error) {
            console.error('Error submitting review:', error);
            alert('An error occurred while submitting the review.');
        }
    });
});
