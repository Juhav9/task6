[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-24ddc0f5d75046c5622901739e7c5dd533143b0c8e959d652212380cedb1ea36.svg)](https://classroom.github.com/a/__BZh1iF)
# Task 06: Web API

<img alt="points bar" align="right" height="36" src="../../blob/badges/.github/badges/points-bar.svg" />

![GitHub Classroom Workflow](../../workflows/GitHub%20Classroom%20Workflow/badge.svg)

***

## Student info

> Write your name, your estimation of how many points you will get, and an estimate of how long it took to make the answer

- Student name: 
- Estimated points: 
- Estimated time (hours): 

***

## Purpose of this task

The purposes of this task are:

- to learn to create an ASP.NET Web api
- to learn to handle routing in web api
- to learn to use models and data with web api
- to learn to understand how JSON and serialization works

## Material for the task

> **Following material will help with the task.**

It is recommended that you will check the material before start coding.

1. [Create web APIs with ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-6.0)
2. [Tutorial: Create a web API with ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-6.0)
3. [Validation attributes](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-6.0#validation-attributes)
4. [Controller action return types in ASP.NET Core web API](https://learn.microsoft.com/en-us/aspnet/core/web-api/action-return-types?view=aspnetcore-6.0)
5. [Loading Related Entities](https://learn.microsoft.com/en-us/ef/ef6/querying/related-data)

## The Task

Create an ASP.NET Web API to handle university course enrollments and grades. Implement basic CRUD (Create, Read, Update and Delete) endpoints to handle the data. Use default ASP.NET conventions for the implementation and use web api with controllers for the endpoints. The data entities (classes) and context are given and properly configured in Program.cs file. Use the given data to implement the endpoints.

### Evaluation points

1. CRUD endpoints for universities. 
   1. Endpoint GET (/university) returns a list of universities. The returned data must contain `university id` and `university name` and be an array of objects containing the required data. No other data is returned.
   2. Endpoint POST (/university) creates a new university. The endpoint must accept following data. Possible value for `id` in the data is ignored and the real id is generated in the backend (in code or database) to ensure the id uniqueness. The endpoint returns 201 created response with `Location` header set to the created resource's uri. The value in the `Location` header is tested with HTTP GET request. The GET request to the URI in the Location header must return the created university's data.
   3. Endpoint DELETE (/university/[id]) deletes the selected university. The university can be deleted only when it has no courses. If the university has course(s) then the delete endpoint does nothing and returns `409 conflict` response. The endpoint returns `404 not found` if there is no university to be found with the provided id. In successful deletion `204 no content` response is returned.
   4. Endpoint GET (/university/make-some-coffee) must return [418 I'm a teapot response](https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/418).

    JSON data for the universities endpoints (comments can be omitted):

    ```json
    {
        "id": 1, // value for university Id
        "name": "Univ. name"
    }
    ```

2. Endpoint GET (/student/[id]/courses) returns a list of courses that the student has enrolled and for completed courses the grades are returned along with the grading date. The endpoint returns `404 not found` if the requested student is not found. The returned data must contain `enrollment id`, `course id`, `course name`, `grade` and `grading date` and be an array of objects containing the required data as (comments can be omitted):

    ```json
    {
        "id": 1, // value for enrollment Id
        "courseId: 3, // value for course Id
        "course": "Course name",
        "grade": 4, // value for grade if any
        "gradingDate": "2024-01-08" // value for grading date if any (must be deserializable to DateTime)
    }
    ```

3. Endpoint PUT (/course/[id]) edits the selected course's data. The name for the course is required and it must not be null or empty string (i.e. it must have content). The endpoint returns `404 not found` if the requested course is not found, `400 bad request` if the request data is invalid (i.e. name does not have a valid value) or `204 no content` when the operation succeeds. After successful edit the data is checked by GET request to the course. The endpoint must accept following data (comments can be omitted):

    ```json
    {
        "name: "course name here"
    }
    ```

4. Endpoint DELETE (/student/[id]/course/[id]) deletes the student's enrollment for the course if the course is not graded. Request to delete a graded course must not change any data and return `409 conflict` response. On successful deletion the endpoint must return `204 no content` response. If the requested enrollment is not found then `404 not found` response is returned. Successful deletion is verified by GET request to student courses endpoint.

5. Endpoint PUT (/grade) sets a grade to the student for a course. `Student id`, `course id`, `grade` and `grading date` is in PUT body. Note that the grade can only be integer value in range [0, 5] (i.e. only values 0, 1, 2, 3, 4 or 5 are allowed). The endpoint must return `400 bad request` if submitted data is not valid. The endpoint returns `204 no content` when the operation succeeds and `404 not found` if the enrollment to be graded is not found. Property `override` (boolean) indicates what to do with possible existing grade. If there is an existing grade and the value is not overridden then `409 conflict` with text content `Has existing value, will not make changes` is returned. The endpoint must accept following data (comments can be omitted):

    ```json
    {
        "studentId": 1, // value for student Id
        "courseId: 3, // value for course Id
        "grade": 3, // value for grade
        "gradingDate": "2024-01-08" // value for grading date (must be deserializable to DateTime),
        "override": true // set to true to replace possible existing grade, set to false to ignore new grade if there is an existing grade
    }
    ```

> **Note!** Read the task description and the evaluation points to get the task's specification (what is required to make the app complete).

## Task evaluation

Evaluation points for the task are described above. An evaluation point either works or does not work there is no "it kind of works" step in between. Be sure to test your work. All working evaluation points are added to the task total and will count toward the course total. The task is worth five (5) points. Each evaluation point is checked individually and each will provide one (1) point so there are five checkpoints. Checkpoints are designed so that they may require additional code, that is not checked or tested, to function correctly.

## DevOps

There is a DevOps pipeline added to this task. The pipeline will build the solution and run automated tests on it. The pipeline triggers when a commit is pushed to GitHub on the main branch. So remember to `git commit` and `git push` when you are ready with the task. The automation uses GitHub Actions and some task runners. The automation is in the folder named .github.

> **DO NOT modify the contents of .github or tests folders.**
