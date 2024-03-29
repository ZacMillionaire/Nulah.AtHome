<script lang="ts">
	import type { ErrorResponse } from 'models/ErrorResponse';
	import ValidationError from '../Form/ValidationError.svelte';
	import type { EventRequestFormModel } from './models/EventRequestFormModel';
	import type { BasicEvent } from './models/BasicEvent';

	let isLoading = false;

	interface FormErrors {
		[field: string]: string[];
	}

	let formErrors: FormErrors;

	// bind the above into a reactive state using svelte syntax
	$: formErrors = {
		Server: [],
		Description: [],
		Start: [],
		End: []
	};

	export let formData: EventRequestFormModel = {};

	export let formSubmitAction: (newEventDescription: EventRequestFormModel) => Promise<void | BasicEvent | Error>;
	/**
	 * Called when formSubmitAction succeeds
	 */
	export let onSuccessCallback: (basicEvent: BasicEvent) => void = null;

	let startInput: HTMLInputElement;
	let endInput: HTMLInputElement;

	function setDateToNow(inputTarget: HTMLInputElement): void {
		const now = new Date();
		now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
		inputTarget.value = now.toISOString().slice(0, 16);
		// Required to ensure svelte/whatever updates formData correctly
		// without this any other input change will clear date fields.
		// It's weird but this was the fix that worked (setting the formData value won't work)
		inputTarget.dispatchEvent(new Event('input'));
	}

	async function formSubmit(): Promise<void> {
		isLoading = true;

		// This is a dumb way to clear errors it but whatever
		formErrors = {
			Server: [],
			Description: [],
			Start: [],
			End: []
		};

		await formSubmitAction(formData)
			.then((eventResult: void | BasicEvent | Error) => {
				isLoading = false;

				if (onSuccessCallback) {
					onSuccessCallback(eventResult as BasicEvent);
				}

				// clear the form
				formData = {};
			})
			.catch((error: Error) => {
				console.log(error);
				// This can be an array or string as internal server errors or other
				const parsedResponse = JSON.parse(error.message) as ErrorResponse[] | string;
				let errorResponse: ErrorResponse[] = [];

				// This block of code is a bit dumb but the parsed response might be a server
				// error which might not be returned in an ErrorResponse (nor will it ever) for reasons
				// such as not wanting to hide exceptions via middleware wrapping it in the backend.
				if (typeof parsedResponse === 'string') {
					errorResponse = [{
						Name: 'Server',
						Description: parsedResponse as string
					}];
				} else {
					errorResponse = parsedResponse as ErrorResponse[];
				}

				errorResponse.forEach(x => {
					formErrors[x.Name].push(x.Description);
				});

				isLoading = false;
			});
	}
</script>

<style lang="scss">
  :root {
    --column-width: 300px;
  }

  .event-form {
    border: 1px solid #c2c2c2;
    border-radius: 5px;
    padding: 5px;
    margin: 5px;
    min-width: var(--column-width);
    box-shadow: 0px 0px 5px 0px rgba(0, 0, 0, 0.15);

    .form-button {
      &.shrink {
        flex: 1; /* fixed width */

      }
    }

    & .form-row {
      margin: 5px 0;
      display: flex;
      flex-direction: row;

      &.shrink-end {
        & :last-child {
          flex: 1; /* fixed width */
        }
      }

      & input, button {
        width: 100%;
        font-size: 16px;
        min-height: 34px;
        border: 1px solid #c2c2c2;
        border-radius: 5px;
      }
    }
  }
</style>

<div class="event-form">
	<ValidationError Errors="{formErrors.Server}" />
	<form on:submit|preventDefault={formSubmit}>
		<div class="form-row">
			<input name="name" id="name" bind:value={formData.Description} required />
		</div>
		<ValidationError Errors="{formErrors.Description}" />
		<div class="form-row">
			<input id="start" type="datetime-local" name="start" bind:value={formData.Start} bind:this={startInput}
						 required />
			<button class="form-button shrink" on:click|preventDefault={()=>	setDateToNow(startInput)}>Now</button>
		</div>
		<ValidationError Errors="{formErrors.Start}" />
		<div class="form-row">
			<input id="end" type="datetime-local" name="end" bind:value={formData.End} bind:this={endInput} />
			<button class="form-button shrink" on:click|preventDefault={()=>setDateToNow(endInput)}>Now</button>
		</div>
		<ValidationError Errors="{formErrors.End}" />
		<div class="form-row">
			<input type="text" name="Tags" placeholder="Comma separated tags for this event" bind:value={formData.Tags} />
		</div>
		<div class="form-row">
			<button disabled={isLoading}>submit</button>
		</div>
	</form>
</div>
