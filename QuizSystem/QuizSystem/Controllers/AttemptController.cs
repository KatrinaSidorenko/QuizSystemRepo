using AutoMapper;
using BLL.Interfaces;
using BLL.Services;
using Core.DTO;
using Core.Enums;
using Core.Models;
using GroupDocs.Viewer.Options;
using GroupDocs.Viewer;
using Microsoft.AspNetCore.Mvc;
using QuizSystem.ViewModels.AttemptViewModels;
using QuizSystem.ViewModels.QuestionViewModel;
using QuizSystem.ViewModels.TakeTestViewModels;
using QuizSystem.ViewModels.PaginationTestViewModels;
using QuizSystem.ViewModels.TestViewModels;

namespace QuizSystem.Controllers
{
    public class AttemptController : Controller
    {
        private readonly ITestService _testService;
        private readonly IAttemptService _attemptService;
        private readonly IQuestionService _questionService;
        private readonly IAnswerService _answerService;
        private readonly ITestResultService _resultService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public AttemptController(IAttemptService attemptService, ITestService testService, IMapper mapper,
            IQuestionService questionService, IAnswerService answerService,
            ITestResultService testResultService, IUserService userService)
        {
            _attemptService = attemptService;
            _testService = testService;
            _questionService = questionService;
            _answerService = answerService;
            _resultService = testResultService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> TakeTest(int testId, int? sharedTestId = null)
        {
            var testResult = await _testService.GetTestById(testId);

            if (!testResult.IsSuccessful)
            {
                TempData["Error"] = testResult.Message;

                return RedirectToAction("Index", "Home");
            }

            var questionsResult = await _questionService.GetTestQuestions(testId);

            var questionsVM = questionsResult.Data.Select(async q =>
            {
                var answers = await _answerService.GetQuestionAnswers(q.QuestionId);

                var answersVm = answers.Data.Select(a =>
                {
                    var answer = _mapper.Map<AnswerTakeTestViewModel>(a);

                    return answer;

                }).ToList();

                var question = _mapper.Map<TakeTestQuestionViewModel>(q);
                question.QuestionAnswers = answersVm;

                return question;

            }).ToList();

            var testVM = _mapper.Map<TakeTestViewModel>(testResult.Data);
            testVM.TakeTestQuestions = Task.WhenAll(questionsVM).Result.ToList();
            testVM.TakedTestUserId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "id")?.Value);

            var attempt = new Attempt()
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                TestId = testId,
                UserId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "id")?.Value),
                SharedTestId = 0
            };

            var attemptId = await _attemptService.AddAttempt(attempt);

            testVM.AttemptId = attemptId.Data;

            if(sharedTestId is not null)
            {
                testVM.SharedTestId = sharedTestId;
            }
            
            return View(testVM);
        }

        [HttpPost]
        public async Task<IActionResult> TakeTest([FromForm] ResultTestViewModel testVM)
        {
            
            var answersDTO = testVM.Answers
                .Select(a => _mapper.Map<AnswerResultDTO>(a)).ToList();

            var testDTO = _mapper.Map<AttemptResultDTO>(testVM);
            testDTO.Answers = answersDTO;

            var answers = testVM.Answers
                .Select(a => _mapper.Map<Answer>(a)).ToList();

            var saveAnswersResult = await _attemptService.SaveUserGivenAnswers(answers, testVM.AttemptId);

            var attemptResult = await _attemptService.SaveAttemptData(testDTO);
          
            return Json(new { redirectUrl = Url.Action("Result", "Attempt", new { attemptId = attemptResult.Data}) });
        }

        [HttpGet]
        public async Task<IActionResult> Result(int attemptId)
        {
            var attempt = await _attemptService.GetAttemptById(attemptId);
            var attemptVm = _mapper.Map<AttemptViewModel>(attempt.Data);

            return View(attemptVm);
        }

        [HttpGet]
        public async Task<IActionResult> AttemptResult(int attemptId)
        {
            var attemptResult = await _attemptService.GetAttemptById(attemptId);

            if (!attemptResult.IsSuccessful)
            {
                TempData["Error"] = "Fail to get attempt";

                return View("Result", attemptId);
            }

            var attemptVm = new AttemptResultViewModel();
            var testResult = await _testService.GetTestById(attemptResult.Data.TestId);

            attemptVm.TestId = testResult.Data.TestId;
            attemptVm.Name = testResult.Data.Name;
            attemptVm.AttemptId = attemptId;

            var questionsResult = await _questionService.GetTestQuestions(testResult.Data.TestId);

            var questionsVM = questionsResult.Data.Select(async q =>
            {
                var answers = await _answerService.GetQuestionAnswers(q.QuestionId);
                

                var answersVm = answers.Data.Select(async a =>
                {
                    var testResult = await _resultService.GetTestResult(attemptId, q.QuestionId, a.AnswerId);
                    var attemptAnswer = new AttemptAnswerViewModel()
                    {
                        AnswerId = a.AnswerId,
                        IsRight = a.IsRight,
                        Value = a.Value
                    };

                    if (testResult.IsSuccessful)
                    {
                        if (q.Type.Equals(QuestionType.Open))
                        {
                            attemptAnswer.ChoosenByUser = testResult.Data.EnteredValue.ToLower().Equals(a.Value) ? true : false;
                            attemptAnswer.ValueByUser = testResult.Data.EnteredValue;
                        }
                        else
                        {
                            attemptAnswer.ChoosenByUser = a.AnswerId == testResult.Data.AnswerId ? true : false;
                        }
                    }
                    else
                    {
                        attemptAnswer.ChoosenByUser = false;
                    }
                          
                    return attemptAnswer;

                }).ToList();

                var question = new AttemptQuestionViewModel()
                {
                    QuestionId = q.QuestionId,
                    Description = q.Description,
                    //GetedPoints = ((float)q.Point / answers.Data.Count) * (float)answersVm.Select(a => a.ChoosenByUser && a.IsRight == true).Count(),
                    Point = (float)q.Point,
                    Type = q.Type,
                    Answers = Task.WhenAll(answersVm).Result.ToList()
                };

                return question;

            }).ToList();

            attemptVm.Questions = Task.WhenAll(questionsVM).Result.ToList();

            return View(attemptVm);
        }

        [HttpGet]
        public async Task<IActionResult> AttemptDocument(int attemptId)
        {
            var pathResult = await _attemptService.GetAttemptDocumentPath(attemptId);

            if (!pathResult.IsSuccessful)
            {
                return RedirectToAction("Index", "Home");
            }

            var outputPath = Path.Combine("Output/Attempts", pathResult.Data.Item1);

            using (Viewer viewer = new Viewer(pathResult.Data.Item2))
            {
                PdfViewOptions options = new PdfViewOptions(outputPath);
                viewer.View(options);
            }

            var fileStream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            var result = new FileStreamResult(fileStream, "application/pdf");

            return result;
        }

        [HttpGet]
        public async Task<IActionResult> Activity(SortingParam sortOrder, int id = 0,  Visibility? filterParam = null, int page = 1, string searchParam = "")
        {
            int userId = id;

            if(userId == 0)
            {
                var temUserId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "id")?.Value);
                var userIdResult = await _userService.IsUserExist(temUserId);

                if (!userIdResult.IsSuccessful)
                {
                    TempData["Error"] = userIdResult.Message;

                    return RedirectToAction("Index", "Home");
                }

                userId = temUserId;
            }
          
            int pageSize = 6;
            string search = string.IsNullOrEmpty(searchParam) ? "" : searchParam.ToLower();

            var activityPaginationModel = new ActivityTestsPaginationModel()
            {
                CurrentPageIndex = page > 0 ? page : 1,
                SearchParam = search,
                UserId = userId,
                SortingParam = sortOrder,
                FilterParam = filterParam
            };
            var testsResult = await _testService.GetUserActivityTests(userId, sortOrder, filterParam,
                page, pageSize, search);

            if (!testsResult.IsSuccessful)
            {
                TempData["Error"] = testsResult.Message;

                return View(activityPaginationModel);
            }


            var testVm = testsResult.Data.Item1.Select(t =>
            {
                var test = _mapper.Map<ActivityViewModel>(t);
                return test;
            }
            );
            var pageCount = (double)((decimal)testsResult.Data.Item2 / Convert.ToDecimal(pageSize));
            activityPaginationModel.Activities = testVm.ToList();
            activityPaginationModel.PageCount = (int)Math.Ceiling(pageCount);
            activityPaginationModel.PageSize = pageSize;

            return View(activityPaginationModel);
        }

        [HttpGet]
        public async Task<IActionResult> History(int testId, int userId, SortingParam sortOrder, FilterParam filterParam, 
            int? sharedTestId = null, int page = 1, string searchParam = "")
        {
            int pageSize = 3;
            string search = string.IsNullOrEmpty(searchParam) ? "" : searchParam.ToLower();

            var attemptViewModel = new AttempyHistoryPaginationModel()
            {
                CurrentPageIndex = page > 0 ? page : 1,
                SearchParam = search,
                UserId = userId,
                TestId = testId,
                SharedTestId = sharedTestId,
                SortingParam = sortOrder,
                FilterParam = filterParam
            };

            var attemptsResult = await _attemptService.GetUserTestAttempts(testId, userId, sortOrder,
                sharedTestId, page, pageSize,
                searchParam, FilterDictionary.FilterParamDict[filterParam].start, 
                FilterDictionary.FilterParamDict[filterParam].end);

            if (!attemptsResult.IsSuccessful)
            {
                TempData["Error"] = attemptsResult.Message;

                return RedirectToAction("Activity", "Attempt");
            }

            var attemptsVm = attemptsResult.Data.Item1.Select(a =>
            {
                var attempt = _mapper.Map<AttemptViewModel>(a);
                return attempt;
            });

            double pageCount;

            if (!string.IsNullOrEmpty(searchParam))
            {
                pageCount = (double)((decimal)attemptsVm.Count() / Convert.ToDecimal(pageSize));
            }
            else
            {
                pageCount = (double)((decimal)attemptsResult.Data.Item2 / Convert.ToDecimal(pageSize));
            }

            attemptViewModel.PageCount = (int)Math.Ceiling(pageCount);
            attemptViewModel.PageSize = pageSize;
            attemptViewModel.Attempts = attemptsVm.ToList();

            return View(attemptViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int testId)
        {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "id")?.Value);

            var userIdResult = await _userService.IsUserExist(userId);

            if (!userIdResult.IsSuccessful)
            {
                TempData["Error"] = userIdResult.Message;

                return RedirectToAction("Index", "Home");
            }

            var statisticResult = await _attemptService.GetTestAttemptsStatistic(testId, userId);

            if (!statisticResult.IsSuccessful)
            {
                TempData["Error"] = statisticResult.Message;

                return RedirectToAction("Index", "Home");
            }

            var statisticVm = _mapper.Map<StatisticViewModel>(statisticResult.Data);

            return View(statisticVm);
            
        }

        [HttpGet]
        public async Task<IActionResult> SharedAttemptsHistory(int sharedTestId, SortingParam sortOrder, int page = 1, string searchParam = "" )
        {
            int pageSize = 3;
            string search = string.IsNullOrEmpty(searchParam) ? "" : searchParam.ToLower();
            
            var attemptViewModel = new AttemptSharedTestPagingModel()
            {
                CurrentPageIndex = page > 0 ? page : 1,
                SearchParam = search,
                SharedTestId = sharedTestId,
                SortingParam = sortOrder
            };

            var attemptsResult = await _attemptService.GetSharedAttempts(sharedTestId, sortOrder, page, pageSize, searchParam);

            if (!attemptsResult.IsSuccessful)
            {
                TempData["Error"] = attemptsResult.Message;

                return RedirectToAction("Index", "SharedTest");
            }

            var attemptsVm = attemptsResult.Data.Item1.Select(a =>
            {
                var attempt = _mapper.Map<AttemptSharedTestResultViewModel>(a);
                return attempt;
            });

            double pageCount;

            if (!string.IsNullOrEmpty(searchParam))
            {
                pageCount = (double)((decimal)attemptsVm.Count() / Convert.ToDecimal(pageSize));
            }
            else
            {
                pageCount = (double)((decimal)attemptsResult.Data.Item2 / Convert.ToDecimal(pageSize));
            }

            attemptViewModel.PageCount = (int)Math.Ceiling(pageCount);
            attemptViewModel.PageSize = pageSize;
            attemptViewModel.UserAttempts = attemptsVm.ToList();

            return View(attemptViewModel);
        }
    }
}
