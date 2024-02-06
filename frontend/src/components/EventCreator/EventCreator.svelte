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
		Description: [],
		Start: [],
		End: []
	};

	export let formData: EventRequestFormModel = {};

	export let formSubmitAction : (newEventDescription: EventRequestFormModel) => Promise<void | BasicEvent | Error>;

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

	async function formSubmit() {
		isLoading = true;

		await formSubmitAction(formData)
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
