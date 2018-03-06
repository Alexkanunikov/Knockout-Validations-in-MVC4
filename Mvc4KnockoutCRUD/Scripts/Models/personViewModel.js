// Ответ ajax - должен быть согласован с enum JsonResponseState
ajaxState = { Ok: 0, Error: -1 };
$(function (section) {
	section.PersonViewModel = function (viewModel) {
		//var model = this;
		var model = ko.mapping.fromJS(viewModel);
		section.basePersonViewModel.apply(model);
		model.IsAddPhoneMode = ko.observable(false);

		section.basePhoneViewModel.apply(model.NewPhoneViewModel);

		//model.NewPhoneViewModel.InitValidation();
		//model.NewPhoneViewModel.Errors = ko.validation.group(model.NewPhoneViewModel);
		model.create = function () {
			if (model.Errors().length > 0) {
				model.Errors.showAllMessages();
				return;
			}
			$.ajax({
				url: section.urls.PersonSave,
				type: 'POST',
				dataType: 'json',
				contentType: "application/json; charset=utf-8",
				data: ko.mapping.toJSON(model), //Seller.Ready 2030
				success: function (response) {

					if (response.State === ajaxState.Ok) {
						alert('ok');
					} else {
						//Выводим сообщения об ошибках
						if (response.Data != null) {
							model.SetUpAllServerErrors(response.Data);
						}
					}
				},
				complete: function () {
				}
			});
		};

		model.AddPhoneDialog = function () {
			model.IsAddPhoneMode(true);
		};

		model.SavePhone = function () {

			if (model.NewPhoneViewModel.Errors().length > 0) {
				model.NewPhoneViewModel.Errors.showAllMessages();
				return;
			} else {
				var dd = ko.mapping.fromJS(ko.toJS(model.NewPhoneViewModel));
				model.Phones.push(dd);
				model.IsAddPhoneMode(false);
			}
		};

		return model;
	};

	// Создание экземпляра модели
	section.PersonViewModel = new section.PersonViewModel(viewModelPerson);
	//section.PersonViewModel.InitValidation();

	//section.PersonViewModel.Errors = ko.validation.group(section.PersonViewModel);

	ko.validation.registerExtenders();
	ko.applyBindings(section.PersonViewModel, $("#personRoot").get(0));//TODO: изменить на 'ko-settings'. сейчас только для css селекторов используется этот id

}(product));