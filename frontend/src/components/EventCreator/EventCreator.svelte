<script lang="ts">
	import { EventStore } from './EventStore';
	import type { NewBasicEventRequest } from './models/NewBasicEventRequest';
	import type { ErrorResponse } from 'models/ErrorResponse';
	import ValidationError from '../Form/ValidationError.svelte';

	let isLoading = false;

	interface BasicEventRequestForm {
		Description: string;
		Tags: string | null;
		Start: Date;
		End: Date | null;
	}

	interface FormErrors {
		[field: string]: string[];
	}

	let formErrors: FormErrors;

	$: formErrors = {
		Description: [],
		Start: [],
		End: []
	};

	let formData: BasicEventRequestForm = {};

	let startInput: HTMLInputElement;
	let endInput: HTMLInputElement;

	function setDateToNow(inputTarget: HTMLInputElement) {
		const now = new Date();
		now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
		inputTarget.value = now.toISOString().slice(0, 16);
		// Required to ensure svelte/whatever updates formData correctly
		// without this any other input change will clear date fields.
		// It's weird but this was the fix that worked (setting the formData value won't work)
		inputTarget.dispatchEvent(new Event('input'));
	}

	async function createEvent(e: SubmitEvent) {
		isLoading = true;

		let newEvent = {
			Description: formData.Description,
			Start: new Date(formData.Start),
			// Make sure that if a user has cleared a form value that we correctly get a null
			// instead of the empty string it will receive.
			// We don't need to do it for Start so long as it is tagged as required in the markup.
			End: formData.End ? new Date(formData.End) : null,
			Tags: formData.Tags?.split(',').map(x => x.trim())
		} as NewBasicEventRequest;

		await EventStore.CreateEvent(newEvent)
			.then(x => {
				console.log(x);
				isLoading = false;
			})
			.catch((error: Error) => {
				const errorResponse = JSON.parse(error.message) as ErrorResponse[];

				// This is a dumb way to clear it but whatever
				formErrors = {
					Description: [],
					Start: [],
					End: []
				};

				errorResponse.forEach(x => {
					formErrors[x.Name].push(x.Description);
				});

				isLoading = false;
			});
	}
</script>

<form on:submit|preventDefault={createEvent}>
	<input name="name" id="name" bind:value={formData.Description} required />
	<ValidationError Errors="{formErrors.Description}" />

	<input id="start" type="datetime-local" name="start" bind:value={formData.Start} bind:this={startInput} required />
	<button on:click|preventDefault={()=>	setDateToNow(startInput)}>Now</button>
	<ValidationError Errors="{formErrors.Start}" />

	<input id="end" type="datetime-local" name="end" bind:value={formData.End} bind:this={endInput} />
	<button on:click|preventDefault={()=>setDateToNow(endInput)}>Now</button>
	<ValidationError Errors="{formErrors.End}" />

	<input type="text" name="Tags" placeholder="Comma separated tags for this event" bind:value={formData.Tags} />
	<button disabled={isLoading}>submit</button>
</form>