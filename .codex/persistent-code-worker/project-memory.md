# Persistent Code Worker Memory

## Project

- Name: ArqSoft-S05-Diego
- Root: /Users/diegoramirezmagana/RiderProjects/ArqSoft-S05-Diego
- Started: 2026-06-23 14:17 UTC
- Last Updated: 2026-06-23 14:17 UTC

## Current Assignment

Implementar práctica 26 Factory y Decorator en rama Api, verificar proyecto completo sin commit/push/pull

## Success Criteria

- [ ] Implement the requested behavior
- [ ] Verify the change with appropriate checks
- [ ] Review the diff and remove partial scaffolding
- [ ] Hand off with risks and blockers clearly stated

## Constraints

- [TODO: Capture product, safety, or technical constraints]

## Important Files

- [TODO: List entry points, configs, tests, and critical modules]

## Architecture Notes

- [TODO: Record relevant system structure and invariants]

## Decisions

- Work only on local branch `Api`; never commit, push, pull, or perform external collaboration actions.
- Follow Practice #26 literally for patients: Production uses an in-memory repository; every other environment uses JSON.
- Keep the existing JSON/CSV selection for doctor and appointment repositories. Patient selection is now environment-based as required by the practice.
- Place new classes in the existing `Repositories/` folder because this branch has no `CitasApp.Infrastructure` project.
- The assignment screenshot makes Observer and REST endpoints mandatory even though the PDF called Observer optional.
- Implement Observer around appointment confirmation, matching the required `POST /api/citas/confirmar/{citaId}` and terminal output shown in the assignment.
- Keep API controllers in the existing web project under `Controllers/Api`; do not create or switch to another project/branch.

## Validation log

- Initial `dotnet build --no-restore` failed with duplicate assembly attributes because ignored `CitasApp.Api/obj` artifacts from another branch were included by the root project's default compile glob.
- Updated `CitasApp.csproj` to exclude the nested `CitasApp.Api` tree from root compilation and content discovery. This makes branch switching safe even when ignored build artifacts remain.
- Development smoke test: `/Paciente` returned HTTP 200, rendered 5 JSON records, and logged `ObtenerTodos` before/after.
- Production smoke test: `/Paciente` returned HTTP 200, rendered 2 in-memory records, and logged `ObtenerTodos` before/after.
- Production CRUD smoke test passed for `ObtenerPorId`, `Agregar`, `Editar`, and `Eliminar`; all operations emitted start/completion logs and the deleted record was absent afterward.
- Fixed `PacienteController.Detalle` to return 404 for the repository's `Id == 0` not-found sentinel.
- Final route smoke test passed: `/`, `/Paciente`, `/Medico`, and `/Cita` returned HTTP 200; `/Paciente/Detalle/999` returned HTTP 404.
- Final Debug verification and Release build both passed with 0 warnings and 0 errors. `git diff --check` passed.
- Observer/API expansion: all required GET endpoints returned 200; missing patient, patient-without-appointments, and missing appointment cases returned 404.
- `POST /api/citas/confirmar/2` returned 200 with `estado: Confirmada` and emitted confirmation, SMS, and email output in the application terminal.
- Restored `Data/Citas.json` after the POST smoke test so delivery data remains unchanged.
- Final automated Debug and Release verification passed with 0 warnings and 0 errors after all Observer/API changes.

- [TODO: Record accepted design choices and why]

## Open Questions

- [TODO: Record unresolved items that may require user input]

## Active Plan

- [TODO: Keep the current step list here]

## Completed Work

- [TODO: Append concrete completed milestones]

## Validation Log

- [TODO: Record tests, lint, manual checks, and outcomes]

## Risks And Blockers

- [TODO: Record known risks, failures, and blockers]

## Restart Notes

- [TODO: Leave enough context so another session can continue fast]
