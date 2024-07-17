import requests

def check_endpoint(url):
    headers = {
        'accept': 'application/json',  # Adjust headers as needed
    }
    try:
        # Disable SSL verification by passing verify=False
        response = requests.get(url, headers=headers, verify=False)
        if response.status_code == 200:
            print("The endpoint is accessible.")
            print("Raw Response Data:", response.content)
            try:
                json_data = response.json()
                print("JSON Response Data:", json_data)
            except ValueError:
                print("Failed to parse JSON. Raw response was:", response.content)
        else:
            print(f"The endpoint returned status code: {response.status_code}")
            print("Response Content:", response.content)
    except requests.exceptions.RequestException as e:
        print(f"An error occurred: {e}")

if __name__ == "__main__":
    url = "https://localhost:7169/api/Product"  # Replace with your actual URL
    check_endpoint(url)
