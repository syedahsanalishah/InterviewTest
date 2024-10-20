// src/EmployeeService.js

const API_BASE_URL = '/list'; // Define the base URL for your API

class EmployeeService {
    static async fetchEmployees() {
        const response = await fetch(API_BASE_URL);
        if (!response.ok) {
            throw new Error('Failed to fetch employees');
        }
        return response.json();
    }

    static async fetchSum() {
        const response = await fetch(`${API_BASE_URL}/sum`);
        if (!response.ok) {
            throw new Error('Failed to fetch sum');
        }
        return response.json();
    }

    static async addEmployee(newEmployee) {
        const response = await fetch(API_BASE_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(newEmployee),
        });
        if (!response.ok) {
            throw new Error('Failed to add employee');
        }
    }

    static async updateEmployee(oldName, updatedEmployee) {
        const response = await fetch(`${API_BASE_URL}/${oldName}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(updatedEmployee),
        });
        if (!response.ok) {
            throw new Error('Failed to update employee');
        }
    }

    static async deleteEmployee(name) {
        const response = await fetch(`${API_BASE_URL}/${name}`, { method: 'DELETE' });
        if (!response.ok) {
            throw new Error('Failed to delete employee');
        }
    }

    static async incrementValues(name) {
        const response = await fetch(`${API_BASE_URL}/increment/${name}`, { method: 'PUT' });
        if (!response.ok) {
            throw new Error('Failed to increment values');
        }
    }
}

export default EmployeeService;
