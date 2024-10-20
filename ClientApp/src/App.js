import React, { useState, useEffect } from 'react';
import EmployeeService from './Services/EmployeeService';
import './App.css'; // Import a separate CSS file if you want to keep styles organized

function App() {
    const [employees, setEmployees] = useState([]);
    const [newEmployee, setNewEmployee] = useState({ name: '', value: 0 });
    const [sumData, setSumData] = useState({ totalSum: null, message: '' });
    const [editableEmployee, setEditableEmployee] = useState(null);
    const [editedValue, setEditedValue] = useState(0);
    const [editedName, setEditedName] = useState('');

    useEffect(() => {
        loadEmployeesAndSum();
    }, []);

    const loadEmployeesAndSum = async () => {
        try {
            const employeesData = await EmployeeService.fetchEmployees();
            setEmployees(employeesData);
            const sumData = await EmployeeService.fetchSum();
            setSumData(sumData);
        } catch (error) {
            console.error('Error fetching data:', error);
        }
    };

    const handleAddEmployee = async () => {
        if (newEmployee.name && newEmployee.value) {
            try {
                await EmployeeService.addEmployee(newEmployee);
                setNewEmployee({ name: '', value: 0 });
                loadEmployeesAndSum(); // Refresh the employee list and sum
            } catch (error) {
                console.error('Error adding employee:', error);
            }
        }
    };

    const handleUpdateEmployee = async (oldName) => {
        try {
            await EmployeeService.updateEmployee(oldName, { name: editedName, value: editedValue });
            setEditableEmployee(null);
            setEditedValue(0);
            setEditedName('');
            loadEmployeesAndSum(); // Refresh the employee list and sum
        } catch (error) {
            console.error('Error updating employee:', error);
        }
    };

    const handleDeleteEmployee = async (name) => {
        try {
            await EmployeeService.deleteEmployee(name);
            loadEmployeesAndSum(); // Refresh the employee list and sum
        } catch (error) {
            console.error('Error deleting employee:', error);
        }
    };

    const handleIncrementValues = async (name) => {
        try {
            await EmployeeService.incrementValues(name);
            loadEmployeesAndSum(); // Refresh the employee list and sum
        } catch (error) {
            console.error('Error incrementing values:', error);
        }
    };

    return (
        <div className="container">
            <h1>Add New Employee</h1>
            <input
                type="text"
                placeholder="Name"
                value={newEmployee.name}
                onChange={(e) => setNewEmployee({ ...newEmployee, name: e.target.value })}
            />
            <input
                type="number"
                placeholder="Value"
                value={newEmployee.value}
                onChange={(e) => setNewEmployee({ ...newEmployee, value: parseInt(e.target.value, 10) })}
            />
            <button onClick={handleAddEmployee}>Add Employee</button>

            <h2>Employee List</h2>

            <table className="employee-table">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Value</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {employees.map(employee => (
                        <tr key={employee.name}>
                            <td>
                                {editableEmployee === employee.name ? (
                                    <input
                                        type="text"
                                        value={editedName}
                                        onChange={(e) => setEditedName(e.target.value)}
                                    />
                                ) : (
                                    employee.name
                                )}
                            </td>
                            <td>
                                {editableEmployee === employee.name ? (
                                    <input
                                        type="number"
                                        value={editedValue}
                                        onChange={(e) => setEditedValue(parseInt(e.target.value, 10))}
                                    />
                                ) : (
                                    employee.value
                                )}
                            </td>
                            <td>
                                {editableEmployee === employee.name ? (
                                    <>
                                        <button onClick={() => handleUpdateEmployee(employee.name)}>
                                            Update
                                        </button>
                                        <button onClick={() => setEditableEmployee(null)}>
                                            Cancel
                                        </button>
                                    </>
                                ) : (
                                    <>
                                        <button onClick={() => {
                                            setEditableEmployee(employee.name);
                                            setEditedName(employee.name);
                                            setEditedValue(employee.value);
                                        }}>
                                            Edit
                                        </button>
                                        <button onClick={() => handleIncrementValues(employee.name)}>
                                            Increment
                                        </button>
                                        <button onClick={() => handleDeleteEmployee(employee.name)}>
                                            Delete
                                        </button>
                                    </>
                                )}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            {sumData.totalSum !== null && (
                <h3>Sum of Values for Names Starting with A, B, or C: {sumData.totalSum}</h3>
            )}
            {sumData.message && (
                <p>{sumData.message}</p>
            )}
        </div>
    );
}

export default App;
